using System.Text.Json.Nodes;
using Eleon.Mcp.Abstractions;
using Eleon.McpSshGateway.Application.Services;

namespace Eleon.McpSshGateway.Mcp.nuke;

public sealed class SshDescribeHostTool : IMcpTool
{
    private static readonly JsonObject Schema = new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["hostId"] = new JsonObject
            {
                ["type"] = "string",
                ["description"] = "Identifier returned by ssh.listHosts"
            }
        },
        ["required"] = new JsonArray { "hostId" }
    };

    private readonly HostCatalogAppService hostCatalog;

    public SshDescribeHostTool(HostCatalogAppService hostCatalog)
    {
        this.hostCatalog = hostCatalog;
    }

    public string Name => "ssh.describeHost";

    public McpToolDescriptor Describe() => new(Name, "Describe a single SSH host and its policy metadata.", Schema);

    public async Task<McpToolCallResult> InvokeAsync(McpToolCallArguments arguments, CancellationToken cancellationToken)
    {
        var input = arguments.Deserialize<DescribeHostInput>() ?? new DescribeHostInput();
        var hostId = input.HostId?.Trim();

        if (string.IsNullOrWhiteSpace(hostId))
        {
            throw new ArgumentException("hostId is required");
        }

        var host = await hostCatalog.GetAsync(hostId, cancellationToken).ConfigureAwait(false);
        if (host is null)
        {
            throw new InvalidOperationException($"Host '{hostId}' was not found.");
        }

        return McpToolCallResult.From(host);
    }

    private sealed record DescribeHostInput
    {
        public string? HostId { get; init; }
    }
}
