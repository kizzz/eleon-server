using System.IO;
using System.Linq;

namespace Eleon.McpGateway.Module.Infrastructure.PathResolvers;

public static class SshBackendPathResolver
{
    public static string Resolve(string? configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.GetFullPath(Environment.ExpandEnvironmentVariables(configuredPath));
        }

        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "..", "Eleon.McpSshGateway.Host.Stdio", "Eleon.McpSshGateway.Host.Stdio.dll"),
            Path.Combine(AppContext.BaseDirectory, "ssh-host", "Eleon.McpSshGateway.Host.Stdio.dll"),
            Path.Combine(AppContext.BaseDirectory, "Eleon.McpSshGateway.Host.Stdio.dll"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "hosts", "Eleon.McpSshGateway.Host.Stdio", "bin", BuildArtifactGuesser.GetCurrentConfiguration(), BuildArtifactGuesser.GetCurrentTargetFramework(), "Eleon.McpSshGateway.Host.Stdio.dll"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "hosts", "Eleon.McpSshGateway.Host.Stdio", "bin", BuildArtifactGuesser.GetCurrentConfiguration(), "Eleon.McpSshGateway.Host.Stdio.dll")
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

