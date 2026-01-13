using Microsoft.Extensions.Logging;
using ServicesOrchestrator.HealthChecks;
using ServicesOrchestrator.Services.Abstractions;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.Services;

[ExposeServices(typeof(IOrchestratingService))]
public sealed class OrchestratingService : IOrchestratingService, ISingletonDependency
{
  private readonly IOrchestratingConfigurationService _cfg;
  private readonly IEnumerable<IServiceHandler> _handlers;
  private readonly ILogger<OrchestratingService> _log;
  private readonly IAlertService _alertService;
  private volatile ServiceCommand _desired = ServiceCommand.Start;
  private readonly Dictionary<string, IServiceHandler> _byType;

  // ---- Runtime state tracking ----
  private sealed class ServiceRuntimeState
  {
    public bool LastUp { get; set; }
    public DateTimeOffset? LastUpAt { get; set; }
    public DateTimeOffset? FirstSeenDownAt { get; set; }
    public int ConsecutiveDownTicks { get; set; }
    public int StartAttempts { get; set; }
    public DateTimeOffset? LastStartAttemptAt { get; set; }

    // Simple ring buffer of recent errors (keep last N)
    private readonly Queue<string> _errors = new();
    private const int MaxErrors = 20;

    public void AddError(string message)
    {
      if (_errors.Count >= MaxErrors) _errors.Dequeue();
      _errors.Enqueue($"{DateTimeOffset.UtcNow:u} {message}");
    }

    public IReadOnlyList<string> GetErrors() => _errors.ToArray();
    public string? LastError => _errors.LastOrDefault();
  }

  private readonly Dictionary<string, ServiceRuntimeState> _state = new(StringComparer.OrdinalIgnoreCase);

  public OrchestratingService(
      IOrchestratingConfigurationService cfg,
      IEnumerable<IServiceHandler> handlers,
      ILogger<OrchestratingService> log,
      IAlertService alertService)
  {
    _cfg = cfg;
    _handlers = handlers;
    _byType = handlers.ToDictionary(h => h.Type, StringComparer.OrdinalIgnoreCase);
    _log = log;
    _alertService = alertService;
  }

  public Task StartAsync(CancellationToken ct)
  {
    _desired = ServiceCommand.Start;
    _log.LogDebug("Orchestration desired state: START.");
    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken ct)
  {
    _desired = ServiceCommand.Stop;
    _log.LogDebug("Orchestration desired state: STOP.");
    return Task.CompletedTask;
  }

  public async Task<int> ProcessAsync(CancellationToken ct)
  {
    var opts = _cfg.Options;
    var services = _cfg.Manifest.Services;

    if (services.Count == 0)
      return opts.DefaultTickMs;

    return _desired == ServiceCommand.Start
        ? await ProcessStartAsync(services, ct)
        : await ProcessStopAsync(services, ct);
  }

  // ---------- Flows ----------

  private async Task<int> ProcessStartAsync(List<ServiceConfig> services, CancellationToken ct)
  {
    var opts = _cfg.Options;
    var ordered = TopoSort(services);
    var anyPending = false;
    var anyAction = false;

    // Pre-probe current up-state (and update runtime)
    var upCache = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
    foreach (var svc in ordered)
    {
      var handler = ResolveHandler(svc);
      var up = await SafeStatusAsync(handler, svc, ct);
      upCache[svc.Name] = up;
      await UpdateRuntimeOnProbe(svc, up);
    }

    // Try to start when deps are up
    foreach (var svc in ordered)
    {
      ct.ThrowIfCancellationRequested();

      // Gate: dependencies must be up
      if (svc.Dependencies.Any(dep => !upCache.TryGetValue(dep, out var depUp) || !depUp))
      {
        anyPending = true;
        continue;
      }

      var isUp = upCache[svc.Name];
      if (!isUp && (svc.Type == "app" || svc.Type == "webapp"))
      {
        var handler = ResolveHandler(svc);
        try
        {
          _log.LogDebug("Starting '{Name}' (type {Type})...", svc.Name, svc.Type);
          await handler.ChangeStatusAsync(svc, ServiceCommand.Start, ct);
          anyAction = true;

          // Record start attempt
          var st = GetState(svc.Name);
          st.StartAttempts++;
          st.LastStartAttemptAt = DateTimeOffset.UtcNow;

          // Optimistic mark; real state will be picked up next tick
          upCache[svc.Name] = true;
        }
        catch (Exception ex)
        {
          var st = GetState(svc.Name);
          st.AddError($"start failed: {ex.Message}");
          _log.LogError(ex, "Start failed for '{Name}'.", svc.Name);
          anyPending = true;
        }
      }
    }

    // After attempting, evaluate prolonged unavailability (only when desired == Start)
    foreach (var svc in ordered)
    {
      await EvaluateProlongedUnavailability(svc);
    }

    return (anyAction || anyPending) ? Math.Max(opts.MinTickMs, 500) : opts.DefaultTickMs;
  }

  private async Task<int> ProcessStopAsync(List<ServiceConfig> services, CancellationToken ct)
  {
    var opts = _cfg.Options;

    var ordered = TopoSort(services);
    ordered = ordered.Reverse().ToList();

    var anyAction = false;
    var anyPending = false;

    foreach (var svc in ordered)
    {
      ct.ThrowIfCancellationRequested();

      var handler = ResolveHandler(svc);
      var isUp = await SafeStatusAsync(handler, svc, ct);

      // Update runtime (stops shouldn’t count as failure)
      await UpdateRuntimeOnProbe(svc, isUp, isStopFlow: true);

      if (isUp)
      {
        try
        {
          _log.LogDebug("Stopping '{Name}' (type {Type})...", svc.Name, svc.Type);
          await handler.ChangeStatusAsync(svc, ServiceCommand.Stop, ct);
          anyAction = true;
        }
        catch (Exception ex)
        {
          var st = GetState(svc.Name);
          st.AddError($"stop failed: {ex.Message}");
          _log.LogWarning(ex, "Stop failed for '{Name}'.", svc.Name);
          anyPending = true;
        }
      }
    }

    return (anyAction || anyPending) ? Math.Max(opts.MinTickMs, 500) : opts.DefaultTickMs;
  }

  // ---------- Status ----------

  public async Task<OrchestratorStatusDto> GetStatusAsync(CancellationToken ct)
  {
    var services = _cfg.Manifest.Services;
    var list = new List<ServiceStatusDto>(services.Count);

    foreach (var svc in services)
    {
      var handler = ResolveHandler(svc);
      var up = await SafeStatusAsync(handler, svc, ct);
      await UpdateRuntimeOnProbe(svc, up); // keep state fresh for UI

      var st = GetState(svc.Name);
      var allowed = svc.AllowedDownMs ?? _cfg.Options.AllowedDownMs;

      var downForMs = st.LastUpAt.HasValue
          ? Math.Max(0, (long)(DateTimeOffset.UtcNow - st.LastUpAt.Value).TotalMilliseconds)
          : (st.LastUp ? 0 : (long)(DateTimeOffset.UtcNow - (st.FirstSeenDownAt ?? DateTimeOffset.UtcNow)).TotalMilliseconds);

      list.Add(new ServiceStatusDto
      {
        Name = svc.Name,
        Type = svc.Type,
        Up = up,
        LastUpAt = st.LastUpAt,
        DownForMs = downForMs,
        StartAttempts = st.StartAttempts,
        ConsecutiveDownTicks = st.ConsecutiveDownTicks,
        LastError = st.LastError,
        RecentErrors = st.GetErrors(),
        Dependencies = svc.Dependencies,
        AllowedDownMs = allowed,
        Desired = _desired.ToString()
      });
    }

    return new OrchestratorStatusDto
    {
      Enabled = _desired == ServiceCommand.Start,
      Services = list
    };
  }

  // ---------- Helpers ----------

  private IServiceHandler ResolveHandler(ServiceConfig svc)
  {
    if (_byType.TryGetValue(svc.Type, out var h)) return h;
    throw new InvalidOperationException($"No IServiceHandler registered for type '{svc.Type}'.");
  }

  private static IReadOnlyList<ServiceConfig> TopoSort(List<ServiceConfig> all)
  {
    var map = all.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);
    var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var temp = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var result = new List<ServiceConfig>();

    bool Dfs(string name)
    {
      if (temp.Contains(name)) return false;
      if (!visited.Add(name)) return true;
      temp.Add(name);
      if (map.TryGetValue(name, out var svc))
      {
        foreach (var d in svc.Dependencies)
          if (map.ContainsKey(d)) Dfs(d);
      }
      temp.Remove(name);
      if (map.TryGetValue(name, out var sv)) result.Add(sv);
      return true;
    }

    foreach (var s in all) Dfs(s.Name);
    return result.DistinctBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList();
  }

  private ServiceRuntimeState GetState(string name)
  {
    if (!_state.TryGetValue(name, out var s))
    {
      s = new ServiceRuntimeState();
      _state[name] = s;
    }
    return s;
  }

  private async Task UpdateRuntimeOnProbe(ServiceConfig svc, bool up, bool isStopFlow = false)
  {
    var st = GetState(svc.Name);
    var now = DateTimeOffset.UtcNow;

    if (up)
    {
      var wasDown = !st.LastUp;
      var downStartedAt = st.FirstSeenDownAt; // capture before we overwrite

      st.LastUp = true;
      st.LastUpAt = now;
      st.ConsecutiveDownTicks = 0;

      // Clear down marker by default when we are up now
      st.FirstSeenDownAt = null;

      // If we were previously down and now recovered — send a recovery alert
      // (but skip during stop flow to avoid noise)
      if (wasDown && !isStopFlow && downStartedAt.HasValue)
      {
        var allowed = svc.AllowedDownMs ?? _cfg.Options.AllowedDownMs;
        var downFor = now - downStartedAt.Value;

        if (allowed > 0 && downFor.TotalMilliseconds >= allowed)
        {
          try
          {
            await _alertService.AlertAsync(new Dictionary<string, string>
                        {
                            { "Message", $"Service '{svc.Name}' recovered after being down for {downFor.TotalMilliseconds} ms (allowed {allowed} ms)." },
                            { "ServiceName", svc.Name },
                            { "ServiceType", svc.Type },
                            { "Recovered", "true" },
                            { "DownForMs", ((long)downFor.TotalMilliseconds).ToString() },
                            { "AllowedDownMs", allowed.ToString() }
                        });
          }
          catch (Exception ex)
          {
            st.AddError($"recovery-alert failed: {ex.Message}");
            _log.LogWarning(ex, "Recovery alert failed for '{Name}'.", svc.Name);
          }
        }
      }
    }
    else
    {
      // Entering or continuing a down state
      if (st.LastUp || st.FirstSeenDownAt is null)
        st.FirstSeenDownAt = now; // mark the first moment we saw it down

      st.LastUp = false;
      st.ConsecutiveDownTicks++;
    }
  }


  /// <summary>
  /// If a service has been down longer than its allowed window while desired==Start
  /// </summary>
  private async Task EvaluateProlongedUnavailability(ServiceConfig svc)
  {
    if (_desired != ServiceCommand.Start) return;

    var st = GetState(svc.Name);
    if (st.LastUp) return;

    var allowed = svc.AllowedDownMs ?? _cfg.Options.AllowedDownMs;
    if (allowed <= 0) return;

    var downFor = st.FirstSeenDownAt.HasValue
        ? DateTimeOffset.UtcNow - st.FirstSeenDownAt.Value
        : TimeSpan.Zero;

    if (downFor.TotalMilliseconds >= allowed)
    {
      try
      {
        await _alertService.AlertAsync(new Dictionary<string, string>()
                {
                    { "Message", $"Service '{svc.Name}' has been down for {downFor.TotalMilliseconds} ms, exceeding allowed {allowed} ms." },
                    { "ServiceName", svc.Name },
                    { "ServiceType", svc.Type },
                    { "DownForMs", ((long)downFor.TotalMilliseconds).ToString() },
                    { "AllowedDownMs", allowed.ToString() }
                });
      }
      catch (Exception ex)
      {
        st.AddError($"prolonged-unavailable hook failed: {ex.Message}");
        _log.LogWarning(ex, "Prolonged-unavailable hook failed for '{Name}'.", svc.Name);
      }
    }
  }

  private async Task<bool> SafeStatusAsync(IServiceHandler handler, ServiceConfig svc, CancellationToken ct)
  {
    try
    {
      return await handler.StatusAsync(svc, ct);
    }
    catch (Exception ex)
    {
      var st = GetState(svc.Name);
      st.AddError($"status failed: {ex.Message}");
      _log.LogWarning(ex, "Status check failed for '{Name}'.", svc.Name);
      return false;
    }
  }
}
