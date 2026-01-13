using ServicesOrchestrator.Services.Abstractions;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.Services.ServiceHandlers;


[ExposeServices(typeof(IServiceHandler))]
public sealed class AppServiceHandler : IServiceHandler, ISingletonDependency
{
  private readonly ServiceLifecycleManager _lifecycle;
  public AppServiceHandler(ServiceLifecycleManager lifecycle) => _lifecycle = lifecycle;

  public string Type => "app";

  public Task<bool> StatusAsync(ServiceConfig svc, CancellationToken ct)
  {
    // Build minimal options (we only need identity to attach)
    var opts = new ManagedServiceOptions
    {
      Name = svc.Name,
      ExecutablePath = svc.ExecutablePath!,   // required for reliable attach by full path/file name
      WorkingDirectory = svc.WorkingDirectory,
      Environment = svc.Env,

      // default timeouts are fine; we don't start here, only attach
    };

    // 1) If already tracked & alive → true
    var current = _lifecycle.GetStatus(svc.Name);
    if (current is ServiceStatus.Running or ServiceStatus.Ready or ServiceStatus.Starting)
      return Task.FromResult(true);

    // 2) Try to attach to an already-running OS process and save to list
    var attached = _lifecycle.EnsureTrackedIfRunning(opts);
    if (attached)
      return Task.FromResult(true);

    // 3) Not tracked and cannot attach → report false
    return Task.FromResult(false);
  }

  public async Task ChangeStatusAsync(ServiceConfig svc, ServiceCommand desired, CancellationToken ct)
  {
    switch (desired)
    {
      case ServiceCommand.Start:
        await _lifecycle.StartAsync(new ManagedServiceOptions
        {
          Name = svc.Name,
          ExecutablePath = svc.ExecutablePath!,
          WorkingDirectory = svc.WorkingDirectory,
          Environment = svc.Env,
          StartTimeout = TimeSpan.FromSeconds(45),
          StopTimeout = TimeSpan.FromSeconds(20)
        }, waitForReady: false, cancellationToken: ct);
        break;

      case ServiceCommand.Stop:
        await _lifecycle.StopAsync(new ManagedServiceOptions
        {
          Name = svc.Name,
          ExecutablePath = svc.ExecutablePath!,
          WorkingDirectory = svc.WorkingDirectory,
          Environment = svc.Env,
          StartTimeout = TimeSpan.FromSeconds(45),
          StopTimeout = TimeSpan.FromSeconds(20)
        }, force: true, cancellationToken: ct);
        break;
    }
  }
}
