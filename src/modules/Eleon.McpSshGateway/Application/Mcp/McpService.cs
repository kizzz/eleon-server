using System.Diagnostics;
using System.Reflection;
using Eleon.Mcp.Abstractions;

namespace Eleon.McpSshGateway.Mcp;

public sealed class McpService
{
    private readonly IMcpToolRegistry registry;
    private readonly IMcpDispatcher dispatcher;
    private readonly string serverVersion;

    public McpService(IMcpToolRegistry registry, IMcpDispatcher dispatcher)
    {
        this.registry = registry;
        this.dispatcher = dispatcher;
        serverVersion = ResolveVersion();
    }

    public Task<McpInitializeResult> InitializeAsync(McpInitializeParams parameters, CancellationToken cancellationToken)
    {
        var info = new McpServerInfo("Eleon MCP SSH Gateway", serverVersion);
        var capabilities = new McpCapabilities(new McpToolCapability { Call = true, List = true });
        return Task.FromResult(new McpInitializeResult(info, capabilities));
    }

    public Task<ToolsListResult> ListToolsAsync(ToolsListParams parameters, CancellationToken cancellationToken)
    {
        var descriptors = registry.Tools.Select(tool => tool.Describe()).ToArray();
        return Task.FromResult(new ToolsListResult(descriptors));
    }

    public async Task<ToolsCallResult> CallToolAsync(ToolsCallParams parameters, CancellationToken cancellationToken)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        var result = await dispatcher.DispatchAsync(parameters.Name, parameters.Arguments, cancellationToken).ConfigureAwait(false);
        return new ToolsCallResult(result.Value);
    }

    private static string ResolveVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        return informationalVersion ?? assembly.GetName().Version?.ToString() ?? "0.0.0";
    }
}
