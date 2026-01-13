using System.Text.Json.Nodes;

namespace Eleon.Mcp.Abstractions;

public interface IMcpTool
{
    string Name { get; }

    McpToolDescriptor Describe();

    Task<McpToolCallResult> InvokeAsync(McpToolCallArguments arguments, CancellationToken cancellationToken);
}

public interface IMcpToolRegistry
{
    IReadOnlyCollection<IMcpTool> Tools { get; }

    IMcpTool? FindByName(string name);
}

public interface IMcpDispatcher
{
    Task<McpToolCallResult> DispatchAsync(string toolName, JsonNode? arguments, CancellationToken cancellationToken);
}
