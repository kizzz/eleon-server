using System.Text.Json.Nodes;
using Eleon.Mcp.Abstractions;
using Eleon.McpSshGateway.Module.Application.Contracts.Dtos;
using Eleon.McpSshGateway.Module.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Eleon.McpSshGateway.Module.Application.Mcp.Tools;

public sealed class SshExecuteTool : IMcpTool
{
    private static readonly JsonObject Schema = new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["hostId"] = new JsonObject
            {
                ["type"] = "string",
                ["description"] = "Identifier of the SSH host"
            },
            ["command"] = new JsonObject
            {
                ["type"] = "string",
                ["description"] = "Shell command to execute remotely"
            },
            ["timeoutSeconds"] = new JsonObject
            {
                ["type"] = "integer",
                ["minimum"] = 1,
                ["maximum"] = 300,
                ["description"] = "Optional timeout override in seconds (default 30, max 300)"
            }
        },
        ["required"] = new JsonArray
        {
            "hostId",
            "command"
        }
    };

    private readonly ISshCommandAppService appService;
    private readonly ILogger<SshExecuteTool> logger;

    public SshExecuteTool(ISshCommandAppService appService, ILogger<SshExecuteTool> logger)
    {
        this.appService = appService;
        this.logger = logger;
    }

    public string Name => "ssh.execute";

    public McpToolDescriptor Describe() => new(Name, "Execute a whitelisted command on a configured SSH host.", Schema);

    public async Task<McpToolCallResult> InvokeAsync(McpToolCallArguments arguments, CancellationToken cancellationToken)
    {
        var payload = arguments.Deserialize<ExecuteCommandInput>() ?? new ExecuteCommandInput();
        var normalized = payload with
        {
            HostId = payload.HostId?.Trim() ?? string.Empty,
            Command = payload.Command?.Trim() ?? string.Empty
        };

        logger.LogInformation("Dispatching ssh.execute for host {HostId}", normalized.HostId);

        var result = await appService.ExecuteAsync(normalized, cancellationToken).ConfigureAwait(false);

        return McpToolCallResult.From(new
        {
            exitCode = result.ExitCode,
            stdout = result.Stdout,
            stderr = result.Stderr,
            durationMs = result.Duration.TotalMilliseconds
        });
    }
}

