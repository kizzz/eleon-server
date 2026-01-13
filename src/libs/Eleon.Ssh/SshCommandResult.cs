namespace Eleon.Ssh;

public sealed record SshCommandResult(int ExitCode, string Stdout, string Stderr, TimeSpan Duration);
