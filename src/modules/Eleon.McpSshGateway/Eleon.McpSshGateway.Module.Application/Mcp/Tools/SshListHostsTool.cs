using System.Text.Json.Nodes;
using Eleon.Mcp.Abstractions;
using Eleon.McpSshGateway.Module.Application.Contracts.Services;

namespace Eleon.McpSshGateway.Module.Application.Mcp.Tools;

public sealed class SshListHostsTool : IMcpTool
{
    private static readonly JsonObject EmptySchema = new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject(),
        ["additionalProperties"] = false
    };

    private readonly IHostCatalogAppService hostCatalog;

    public SshListHostsTool(IHostCatalogAppService hostCatalog)
    {
        this.hostCatalog = hostCatalog;
    }

    public string Name => "ssh.listHosts";

    public McpToolDescriptor Describe() => new(Name, "List the SSH hosts that this MCP gateway can access.", EmptySchema);

    public async Task<McpToolCallResult> InvokeAsync(McpToolCallArguments arguments, CancellationToken cancellationToken)
    {
        var hosts = await hostCatalog.ListAsync(cancellationToken).ConfigureAwait(false);
        return McpToolCallResult.From(new { hosts });
    }
}

