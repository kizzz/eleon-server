namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class McpSessionOptions
{
    public TimeSpan SessionTtl { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);
}

