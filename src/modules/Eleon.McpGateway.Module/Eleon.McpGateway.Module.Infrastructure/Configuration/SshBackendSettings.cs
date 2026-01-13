using System.IO;
using Eleon.Mcp.Infrastructure.Utilities;
using Eleon.McpGateway.Module.Infrastructure.PathResolvers;
using Microsoft.Extensions.Configuration;

namespace Eleon.McpGateway.Module.Infrastructure.Configuration;

public sealed record SshBackendSettings(
    string ExecutablePath,
    IReadOnlyList<string> AdditionalArguments,
    string WorkingDirectory)
{
    public static SshBackendSettings Create(IConfiguration configuration)
    {
        var executablePath = SshBackendPathResolver.Resolve(configuration["MCP_BACKEND_SSH_PATH"]);
        var args = CommandLineTokenizer.Tokenize(configuration["MCP_BACKEND_SSH_ARGS"]);
        var workingDirectory = ResolveWorkingDirectory(configuration["MCP_BACKEND_SSH_WORKING_DIR"], executablePath);
        return new SshBackendSettings(executablePath, args, workingDirectory);
    }

    private static string ResolveWorkingDirectory(string? configured, string executablePath)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured);
        }

        var directory = Path.GetDirectoryName(executablePath);
        return string.IsNullOrWhiteSpace(directory) ? AppContext.BaseDirectory : directory;
    }
}

