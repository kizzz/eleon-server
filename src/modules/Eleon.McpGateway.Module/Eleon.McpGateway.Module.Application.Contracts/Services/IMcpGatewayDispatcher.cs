using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Domain;
using Volo.Abp.Application.Services;

namespace Eleon.McpGateway.Module.Application.Contracts.Services;

public interface IMcpGatewayDispatcher : IApplicationService
{
    string ResolveBackendNameOrDefault(string? backendName);
    void EnsureBackendExists(string backendName);
    Task ForwardAsync(JsonNode payload, string backendName, CancellationToken cancellationToken);
    IAsyncEnumerable<JsonNode> GetOutboundStream(string backendName, CancellationToken cancellationToken);
    
    // Session-aware overloads
    Task ForwardAsync(JsonNode payload, string backendName, string? sessionId, CancellationToken cancellationToken);
    IAsyncEnumerable<JsonNode> GetOutboundStream(string backendName, string? sessionId, CancellationToken cancellationToken);
}

