using System.IO;
using System.Linq;

namespace Eleon.McpGateway.Module.Infrastructure.PathResolvers;

public static class CodexBackendPathResolver
{
    public static string Resolve(string? configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.GetFullPath(Environment.ExpandEnvironmentVariables(configuredPath));
        }

        var candidates = new[]
        {
            // Same bin/Debug/hosts/* layout as the SSE host itself.
            Path.Combine(AppContext.BaseDirectory, "..", "Eleon.McpCodexGateway.Host.Stdio", "Eleon.McpCodexGateway.Host.Stdio.dll"),

            Path.Combine(AppContext.BaseDirectory, "codex-host", "Eleon.McpCodexGateway.Host.Stdio.dll"),
            Path.Combine(AppContext.BaseDirectory, "Eleon.McpCodexGateway.Host.Stdio.dll"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "hosts", "Eleon.McpCodexGateway.Host.Stdio", "bin", BuildArtifactGuesser.GetCurrentConfiguration(), BuildArtifactGuesser.GetCurrentTargetFramework(), "Eleon.McpCodexGateway.Host.Stdio.dll"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "hosts", "Eleon.McpCodexGateway.Host.Stdio", "bin", BuildArtifactGuesser.GetCurrentConfiguration(), "Eleon.McpCodexGateway.Host.Stdio.dll")
        };

        foreach (var candidate in candidates.Select(path => Path.GetFullPath(path)))
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return Path.GetFullPath(candidates.Last());
    }
}

