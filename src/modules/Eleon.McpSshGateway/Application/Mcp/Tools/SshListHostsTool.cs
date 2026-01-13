using System.Text.Json.Nodes;
using Eleon.Mcp.Abstractions;
using Eleon.McpSshGateway.Application.Services;

namespace Eleon.McpSshGateway.Mcp.nuke;

public sealed class SshListHostsTool : IMcpTool
{
    private static readonly JsonObject EmptySchema = new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject(),
        ["additionalProperties"] = false
    };

    private readonly HostCatalogAppService hostCatalog;

    public SshListHostsTool(HostCatalogAppService hostCatalog)
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
