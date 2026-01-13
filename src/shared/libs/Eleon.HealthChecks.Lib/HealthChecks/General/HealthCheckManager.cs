using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedModule.modules.Helpers.Module;
using System.Diagnostics;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;

public class HealthCheckItem
{
  public bool Collecting { get; set; } = true;
  public bool Delivering { get; set; } = false;
  public int DeliveringFailsCount { get; set; } = 0;
  public bool DeliveringFailed { get; set; } = false;
  public bool DeliveringSuccessful { get; set; } = false;
  public required HealthCheckEto HealthCheck { get; set; }
}

public class HealthCheckManager // singleton
{
  private readonly IServiceProvider _serviceProvider;
  private readonly IHealthCheckService _healthCheckService;
  private readonly ILogger<HealthCheckManager> _logger;
  private readonly HealthCheckOptions _options;

  private HealthCheckItem _latest;

  private List<HealthCheckReportEto> _globalReports = new();

  private bool _isExecuting = false;

  public HealthCheckManager(
      IServiceProvider serviceProvider,
      IOptions<HealthCheckOptions> options,
      IHealthCheckService healthCheckService,
      ILogger<HealthCheckManager> logger)
  {
    _serviceProvider = serviceProvider;
    _healthCheckService = healthCheckService;
    _logger = logger;
    _options = options.Value;
  }

  public void AddGlobalReport(HealthCheckReportEto report)
  {
    if (_globalReports.Any(r => r.CheckName == report.CheckName))
    {
      _globalReports.RemoveAll(r => r.CheckName == report.CheckName);
    }

    report.ServiceName = _options.ApplicationName;
    report.ServiceVersion = VersionHelper.Version;
    report.UpTime = StartupDiagnostics.GetUptime();
    _globalReports.Add(report);
  }

  public async Task AddReportAsync(HealthCheckReportEto report)
  {
    if (_latest == null)
    {
      _logger.LogWarning("No latest health check to add report to.");
      return;
    }

    report.ServiceName = _options.ApplicationName;
    _latest.HealthCheck.Reports.Add(report);
    if (_latest.DeliveringSuccessful)
    {
      var success = await _healthCheckService.AddReportAsync(report);
      if (!success)
      {
        _logger.LogError("Failed to send report");
      }
    }
  }

  public HealthCheckItem GetLatestHealthCheck()
  {
    return _latest ?? new HealthCheckItem { HealthCheck = new HealthCheckEto() };
  }

  public async Task ExecuteHealthCheckAsync(string type, string initiator, Guid? healthCheckId = null)
  {
    if (!_options.Enabled)
    {
      return;
    }

    if (_isExecuting)
    {
      return;
    }

    _isExecuting = true;

    try
    {
      await ProcessHealthCheckAsync(healthCheckId, type, initiator);
      if (_options.SendImmediately)
      {
        await SendHealthCheckAsync();
      }
    }
    finally
    {
      _isExecuting = false;
    }
  }

  private async Task<HealthCheckEto> ProcessHealthCheckAsync(Guid? id, string type, string initiator)
  {
    var healthCheck = new HealthCheckEto
    {
      Id = id ?? Guid.Empty,
      CreationTime = DateTime.UtcNow,
      Type = type,
      InitiatorName = initiator,
      Reports = new List<HealthCheckReportEto>(_globalReports),
    };

    var item = new HealthCheckItem
    {
      DeliveringSuccessful = false,
      HealthCheck = healthCheck,
    };

    _latest = item;
    await RunHealthChecksAsync(item);
    _latest.Collecting = false;
    return healthCheck;
  }

  private async Task RunHealthChecksAsync(HealthCheckItem item, CancellationToken ct = default)
  {
    using var scope = _serviceProvider.CreateScope();
    var healthChecks = scope.ServiceProvider.GetServices<IEleonsoftHealthCheck>().ToList();

    if (_options.Enabled && _options.EnabledChecks != null && _options.EnabledChecks.Count > 0 && !_options.EnabledChecks.Contains("*"))
    {
      healthChecks = healthChecks.Where(hc => _options.EnabledChecks.Contains(hc.Name)).ToList();
    }

    // Create placeholders first (Status = InProgress), add them to the lists,
    // then run all checks in parallel and update those same objects in place.
    var tasks = new List<Task>(healthChecks.Count);

    foreach (var hc in healthChecks)
    {
      var placeholder = new HealthCheckReportEto
      {
        Status = HealthCheckStatus.InProgress, // make sure enum has this
        Message = "Running...",
        ServiceName = _options.ApplicationName,
        ServiceVersion = VersionHelper.Version,
        UpTime = StartupDiagnostics.GetUptime(),
        HealthCheckId = item.HealthCheck.Id,
        CheckName = hc.Name,
        IsPublic = hc.IsPublic,
        ExtraInformation = new List<ReportExtraInformationEto>
                {
                    new ReportExtraInformationEto
                    {
                        Key = "StartedAtUtc",
                        Value = DateTime.UtcNow.ToString("o"),
                        Severity = ReportInformationSeverity.Info,
                    }
                },
      };

      // Add placeholders synchronously so UI can show them immediately.
      // If item.HealthCheck.Reports can be read from other threads, consider locking or returning snapshots.
      item.HealthCheck.Reports.Add(placeholder);

      tasks.Add(RunSingleAsync(hc, placeholder, item.HealthCheck.Id, ct));
    }

    tasks.Add(CollectMicrosoftHealthChecks(item.HealthCheck.Id));

    await Task.WhenAll(tasks).ConfigureAwait(false);

    async Task RunSingleAsync(IEleonsoftHealthCheck hc, HealthCheckReportEto target, Guid healthCheckId, CancellationToken token)
    {
      var sw = Stopwatch.StartNew();
      try
      {
        // Your IHealthCheck returns a full report; we copy fields into the existing object
        // to avoid replacing references that the UI may already hold.
        var checkTask = hc.CheckAsync(healthCheckId);
        await Task.WhenAll(checkTask, Task.Delay(_options.CheckTimeout * 1000, token)).ConfigureAwait(false);
        var res = await checkTask;

        target.Status = res.Status;
        target.Message = res.Message;

        if (res.ExtraInformation != null)
        {
          foreach (var kv in res.ExtraInformation)
            target.ExtraInformation.Add(kv);
        }
      }
      catch (OperationCanceledException) when (token.IsCancellationRequested)
      {
        target.Status = HealthCheckStatus.Failed; // or a special Canceled state if you have one
        target.Message = "Canceled";
      }
      catch (Exception ex)
      {
        target.Status = HealthCheckStatus.Failed;
        target.Message = ex.Message;
        target.ExtraInformation.Add(new ReportExtraInformationEto { Key = "Exception", Value = ex.ToString(), Severity = ReportInformationSeverity.Error });
        target.ExtraInformation.Add(new ReportExtraInformationEto { Key = "StackTrace", Value = ex.StackTrace ?? "", Severity = ReportInformationSeverity.Error });
      }
      finally
      {
        sw.Stop();
        target.ExtraInformation.Add(new ReportExtraInformationEto { Key = "FinishedAtUtc", Value = DateTime.UtcNow.ToString("o") });
        target.ExtraInformation.Add(new ReportExtraInformationEto { Key = "DurationMs", Value = sw.ElapsedMilliseconds.ToString() });
      }
    }
  }

  internal async Task SendHealthCheckAsync(CancellationToken cancellationToken = default)
  {
    var SendReportCheckName = "HealthCheckDelivery";

    try
    {
      if (_latest == null)
      {
        _logger.LogWarning("No latest health check to send.");
        return;
      }

      var latestSnapshot = _latest; // to avoid changes during execution

      if (latestSnapshot.DeliveringSuccessful)
      {
        return;
      }

      if (latestSnapshot.Collecting)
      {
        return;
      }

      if (latestSnapshot.Delivering)
      {
        return;
      }

      latestSnapshot.Delivering = true;

      try
      {
        var response = await _healthCheckService.SendHealthCheckAsync(latestSnapshot.HealthCheck, cancellationToken);

        if (response.Success)
        {
          latestSnapshot.DeliveringSuccessful = true;
          latestSnapshot.DeliveringFailsCount = 0;
          latestSnapshot.DeliveringFailed = false;
          latestSnapshot.HealthCheck.Id = response.HealthCheckId;

          if (latestSnapshot.HealthCheck?.Reports == null || latestSnapshot.HealthCheck.Reports.Count == 0)
          {
            _logger.LogWarning("No reports found in the health check after successful delivery.");
          }
          else if (latestSnapshot.HealthCheck.Reports.Any(r => r.Status == HealthCheckStatus.Failed))
          {
            var failedReports = latestSnapshot.HealthCheck.Reports.Where(x => x.Status == HealthCheckStatus.Failed).ToArray();
            _logger.LogError(
                "Health check sent successfully but contains failed reports for service {applicationName}. Errored reports ({count}): {erroredReports}",
                _options.ApplicationName,
                failedReports.Count(),
                string.Join(";\n", failedReports.Select(x => $"{x.CheckName} ({x.Message})"))
                );
          }
        }
        else
        {
          latestSnapshot.DeliveringFailed = true;
          latestSnapshot.DeliveringFailsCount++;

          _logger.LogError("Failed to send health check for service {applicationName}. Fails count: {failesCount}. Error: {error}", _options.ApplicationName, latestSnapshot.DeliveringFailsCount, response.Error);

          if (latestSnapshot.HealthCheck.Reports.Any(r => r.CheckName == SendReportCheckName))
          {
            var report = latestSnapshot.HealthCheck.Reports.First(r => r.CheckName == SendReportCheckName);

            report.Message = GetMessage(latestSnapshot.DeliveringFailsCount);

            var lastError = report.ExtraInformation.FirstOrDefault(e => e.Key == "LastError");

            if (lastError != null)
            {
              lastError.Value = response.Error;
            }
            else
            {
              report.ExtraInformation.Add(new ReportExtraInformationEto
              {
                Key = "LastError",
                Value = response.Error,
                Severity = ReportInformationSeverity.Error,
              });
            }

            var failsCount = report.ExtraInformation.FirstOrDefault(e => e.Key == "FailsCount");
            if (failsCount != null)
            {
              failsCount.Value = latestSnapshot.DeliveringFailsCount.ToString();
            }
            else
            {
              report.ExtraInformation.Add(new ReportExtraInformationEto
              {
                Key = "FailsCount",
                Value = latestSnapshot.DeliveringFailsCount.ToString(),
                Severity = ReportInformationSeverity.Info,
              });
            }

            report.ExtraInformation.Add(new ReportExtraInformationEto
            {
              Key = "AttemptedAtUtc",
              Value = DateTime.UtcNow.ToString("o"),
              Severity = ReportInformationSeverity.Info,
            });

            return;
          }
          else
          {
            latestSnapshot.HealthCheck.Reports.Add(new HealthCheckReportEto
            {
              Status = HealthCheckStatus.Failed,
              Message = GetMessage(latestSnapshot.DeliveringFailsCount),
              ServiceName = _options.ApplicationName,
              ServiceVersion = VersionHelper.Version,
              UpTime = StartupDiagnostics.GetUptime(),
              HealthCheckId = latestSnapshot.HealthCheck.Id,
              CheckName = SendReportCheckName,
              IsPublic = false,
              ExtraInformation = new List<ReportExtraInformationEto>
                        {
                            new ReportExtraInformationEto
                            {
                                Key = "LastError",
                                Value = response.Error,
                                Severity = ReportInformationSeverity.Error,
                            },
                            new ReportExtraInformationEto
                            {
                                Key = "AttemptedAtUtc",
                                Value = DateTime.UtcNow.ToString("o"),
                                Severity = ReportInformationSeverity.Info,
                            },
                            new ReportExtraInformationEto
                            {
                                Key = "FailsCount",
                                Value = latestSnapshot.DeliveringFailsCount.ToString(),
                                Severity = ReportInformationSeverity.Info,
                            },
                        },
            });
          }
        }
      }
      finally
      {
        latestSnapshot.Delivering = false;
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, GetMessage(_latest?.DeliveringFailsCount ?? -1));
    }

    static string GetMessage(int count)
    {
      return $"Failed to send health check {count} times";
    }
  }

  private async Task<List<HealthCheckReportEto>> CollectMicrosoftHealthChecks(Guid checkId)
  {
    try
    {
      using var scope = _serviceProvider.CreateScope();
      var healthCheckService = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>();
      var reports = new List<HealthCheckReportEto>();
      var result = await healthCheckService.CheckHealthAsync();
      foreach (var entry in result.Entries)
      {
        var status = entry.Value.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy
            ? HealthCheckStatus.OK
            : entry.Value.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded
                ? HealthCheckStatus.OK
                : HealthCheckStatus.Failed;
        var healthCheckReport = new HealthCheckReportEto
        {
          CheckName = entry.Key,
          Status = status,
          HealthCheckId = checkId,
          ServiceName = _options.ApplicationName,
          IsPublic = true,
          UpTime = StartupDiagnostics.GetUptime(),
          ServiceVersion = VersionHelper.Version,
          Message = entry.Value.Description ?? entry.Value.Status.ToString(),
          ExtraInformation = entry.Value.Data.Select(kv => new ReportExtraInformationEto
          {
            Key = kv.Key,
            Value = kv.Value?.ToString() ?? "null",
            Severity = ReportInformationSeverity.Info,
          }).ToList(),
        };
        reports.Add(healthCheckReport);
      }

      return reports;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error collecting Microsoft health checks");
      return new List<HealthCheckReportEto>();
    }
  }
}
