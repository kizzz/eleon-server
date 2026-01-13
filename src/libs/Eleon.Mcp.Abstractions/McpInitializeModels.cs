namespace Eleon.Mcp.Abstractions;

public sealed record McpInitializeParams(string ClientName, string ClientVersion);

public sealed record McpInitializeResult(McpServerInfo ServerInfo, McpCapabilities Capabilities);

public sealed record McpServerInfo(string Name, string Version);

public sealed record McpCapabilities
{
    public McpCapabilities()
    {
    }

    public McpCapabilities(McpToolCapability tools)
    {
        Tools = tools;
    }

    public McpToolCapability Tools { get; init; } = new();
}

public sealed record McpToolCapability
{
    public bool List { get; init; } = true;

    public bool Call { get; init; } = true;
}
