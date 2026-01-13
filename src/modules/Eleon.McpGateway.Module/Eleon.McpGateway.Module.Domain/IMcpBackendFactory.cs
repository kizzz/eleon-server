namespace Eleon.McpGateway.Module.Domain;

public interface IMcpBackendFactory
{
    Task<IMcpBackend> CreateAsync(string backendName, CancellationToken cancellationToken);
}

