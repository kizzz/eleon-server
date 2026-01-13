using Microsoft.Data.SqlClient;
using ServicesOrchestrator.Services.Abstractions;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.Services.ServiceHandlers;

[ExposeServices(typeof(IServiceHandler))]
public sealed class DatabaseServiceHandler : IServiceHandler, ISingletonDependency
{
  public string Type => "database";

  public async Task<bool> StatusAsync(ServiceConfig svc, CancellationToken ct)
  {
    if (string.IsNullOrWhiteSpace(svc.ConnectionString)) return false;
    try
    {
      using var c = new SqlConnection(svc.ConnectionString);
      using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
      cts.CancelAfter(TimeSpan.FromSeconds(3));
      await c.OpenAsync(cts.Token);
      return true;
    }
    catch { return false; }
  }

  public Task ChangeStatusAsync(ServiceConfig svc, ServiceCommand desired, CancellationToken ct)
  {
    // NO-OP for external DB
    return Task.CompletedTask;
  }
}
