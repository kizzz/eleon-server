namespace Eleon.McpSshGateway.Module.Application.Contracts.Dtos;

public sealed record ExecuteCommandInput
{
    public string HostId { get; init; } = string.Empty;

    public string Command { get; init; } = string.Empty;

    public int? TimeoutSeconds { get; init; }
}

