using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class McpSessionState
{
    public required string SessionId { get; init; }
    public required string BackendName { get; init; }
    public required IMcpBackend Backend { get; init; }
    public ConcurrentDictionary<string, TaskCompletionSource<JsonNode>> PendingRequests { get; } = new();
    public ConcurrentDictionary<string, JsonNode> EarlyResponses { get; } = new();
    public ConcurrentDictionary<string, byte> CompletedRequests { get; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime LastAccessedAt { get; set; }
    public SemaphoreSlim Lock { get; } = new(1, 1);
}
