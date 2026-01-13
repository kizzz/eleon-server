namespace ServicesOrchestrator.Services.Abstractions;

public interface IServiceHandler
{
  /// Handler type key: "database" | "connection" | "app" | "webapp"
  string Type { get; }

  /// Returns true if the service is currently up/healthy.
  Task<bool> StatusAsync(ServiceConfig svc, CancellationToken ct);

  /// Changes the service state. For database/connection, this is a NO-OP.
  Task ChangeStatusAsync(ServiceConfig svc, ServiceCommand desired, CancellationToken ct);
}
