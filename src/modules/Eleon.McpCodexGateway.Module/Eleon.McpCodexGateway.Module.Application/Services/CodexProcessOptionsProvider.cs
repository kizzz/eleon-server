using Eleon.Mcp.Infrastructure.Utilities;
using Eleon.McpCodexGateway.Module.Application.Contracts.Services;
using Eleon.McpCodexGateway.Module.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eleon.McpCodexGateway.Module.Application.Services;

public sealed class CodexProcessOptionsProvider(
    IConfiguration configuration,
    ILogger<CodexProcessOptionsProvider> logger) : ICodexProcessOptionsProvider
{
    private const string WorkspaceVariable = "CODEX_WORKSPACE_DIR";
    private const string SandboxVariable = "CODEX_SANDBOX_MODE";
    private const string ExtraArgsVariable = "CODEX_EXTRA_ARGS";
    private static readonly string DefaultWorkspace = "/workspace";
    private static readonly string DefaultSandbox = "workspace-write";

    public CodexProcessOptions GetOptions()
    {
        var workspace = GetNormalizedPath(configuration[WorkspaceVariable], DefaultWorkspace);
        var sandboxMode = string.IsNullOrWhiteSpace(configuration[SandboxVariable])
            ? DefaultSandbox
            : configuration[SandboxVariable]!;
        var extraArguments = CommandLineTokenizer.Tokenize(configuration[ExtraArgsVariable]);
        logger.LogInformation("Preparing Codex process (workspace: {Workspace}, sandbox: {SandboxMode}, extraArgs: {ExtraArgsCount})",
            workspace,
            sandboxMode,
            extraArguments.Count);
        return new CodexProcessOptions(workspace, sandboxMode, extraArguments);
    }

    private static string GetNormalizedPath(string? value, string fallback)
    {
        var resolved = string.IsNullOrWhiteSpace(value) ? fallback : value!;
        try
        {
            return Path.GetFullPath(resolved);
        }
        catch (Exception)
        {
            return fallback;
        }
    }
}

