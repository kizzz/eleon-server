namespace Eleon.McpCodexGateway.Module.Domain.ValueObjects;

public sealed record CodexProcessOptions(
    string WorkspaceDirectory,
    string SandboxMode,
    IReadOnlyList<string> ExtraArguments);

