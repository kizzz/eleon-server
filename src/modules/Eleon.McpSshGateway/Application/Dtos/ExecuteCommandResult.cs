namespace Eleon.McpSshGateway.Application.Dtos;

public sealed record ExecuteCommandResult(int ExitCode, string Stdout, string Stderr, TimeSpan Duration);
