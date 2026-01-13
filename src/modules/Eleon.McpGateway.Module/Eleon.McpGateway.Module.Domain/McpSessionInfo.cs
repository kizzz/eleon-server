namespace Eleon.McpGateway.Module.Domain;

public sealed record McpSessionInfo
{
    public required string SessionId { get; init; }
    public required string BackendName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime LastAccessedAt { get; init; }
}

