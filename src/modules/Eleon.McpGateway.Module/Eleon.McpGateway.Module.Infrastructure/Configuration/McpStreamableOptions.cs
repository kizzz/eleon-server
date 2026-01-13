namespace Eleon.McpGateway.Module.Infrastructure.Configuration;

public sealed class McpStreamableOptions
{
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool TolerantMode { get; set; } = false;
    public TimeSpan SseKeepaliveInterval { get; set; } = TimeSpan.FromSeconds(15);
    public IReadOnlyList<string> AllowedOrigins { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ExposedHeaders { get; set; } = new[] { "Mcp-Session-Id" };
}

