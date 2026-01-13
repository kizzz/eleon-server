using System.IO;

namespace Eleon.McpSshGateway.Module.Infrastructure.HostCatalog;

public sealed class FileHostRepositoryOptions
{
    public string CatalogPath { get; set; } = ResolveDefaultPath();

    private static string ResolveDefaultPath()
    {
        var envOverride = Environment.GetEnvironmentVariable("ELEON_MCP_HOSTS_PATH");
        if (!string.IsNullOrWhiteSpace(envOverride))
        {
            return Path.GetFullPath(envOverride);
        }

        var current = AppContext.BaseDirectory;
        for (var i = 0; i < 10 && !string.IsNullOrEmpty(current); i++)
        {
            var candidate = Path.Combine(current, ".agents", "logs", "meta", "mcp", "hosts.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        return Path.Combine(AppContext.BaseDirectory, "hosts.json");
    }
}
