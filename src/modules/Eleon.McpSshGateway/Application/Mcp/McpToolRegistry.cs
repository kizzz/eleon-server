using Eleon.Mcp.Abstractions;

namespace Eleon.McpSshGateway.Mcp;

public sealed class McpToolRegistry : IMcpToolRegistry
{
    private readonly IReadOnlyCollection<IMcpTool> tools;
    private readonly IReadOnlyDictionary<string, IMcpTool> lookup;

    public McpToolRegistry(IEnumerable<IMcpTool> tools)
    {
        this.tools = (tools ?? Array.Empty<IMcpTool>()).ToArray();
        lookup = this.tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<IMcpTool> Tools => tools;

    public IMcpTool? FindByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return lookup.TryGetValue(name, out var tool) ? tool : null;
    }
}
