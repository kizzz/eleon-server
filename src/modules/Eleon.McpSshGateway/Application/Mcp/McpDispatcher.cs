using System.Text.Json.Nodes;
using Eleon.Mcp.Abstractions;
using Microsoft.Extensions.Logging;

namespace Eleon.McpSshGateway.Mcp;

public sealed class McpDispatcher : IMcpDispatcher
{
    private readonly IMcpToolRegistry registry;
    private readonly ILogger<McpDispatcher> logger;

    public McpDispatcher(IMcpToolRegistry registry, ILogger<McpDispatcher> logger)
    {
        this.registry = registry;
        this.logger = logger;
    }

    public async Task<McpToolCallResult> DispatchAsync(string toolName, JsonNode? arguments, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(toolName))
        {
            throw new ArgumentException("Tool name is required", nameof(toolName));
        }

        var tool = registry.FindByName(toolName) ?? throw new InvalidOperationException($"Tool '{toolName}' is not registered.");
        logger.LogInformation("Invoking MCP tool {Tool}", toolName);
        return await tool.InvokeAsync(new McpToolCallArguments(arguments), cancellationToken).ConfigureAwait(false);
    }
}
