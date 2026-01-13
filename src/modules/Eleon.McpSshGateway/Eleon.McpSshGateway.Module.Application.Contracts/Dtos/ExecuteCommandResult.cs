namespace Eleon.McpSshGateway.Module.Application.Contracts.Dtos;

public sealed record ExecuteCommandResult(int ExitCode, string Stdout, string Stderr, TimeSpan Duration);

