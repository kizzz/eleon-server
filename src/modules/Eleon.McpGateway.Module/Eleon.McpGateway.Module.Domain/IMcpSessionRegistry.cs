namespace Eleon.McpGateway.Module.Domain;

public interface IMcpSessionRegistry
{
    Task<McpSessionInfo> GetOrCreateAsync(string? sessionId, string backendName, CancellationToken cancellationToken);

    Task TouchAsync(string sessionId, CancellationToken cancellationToken);

    Task TerminateAsync(string sessionId, CancellationToken cancellationToken);

    Task<McpSessionInfo?> TryGetAsync(string sessionId, CancellationToken cancellationToken);
    
    Task<IMcpBackend> GetBackendAsync(string sessionId, CancellationToken cancellationToken);
}

