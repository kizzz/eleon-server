using System.IO;
using Eleon.Mcp.Infrastructure.Utilities;
using Eleon.McpGateway.Module.Infrastructure.PathResolvers;
using Microsoft.Extensions.Configuration;

namespace Eleon.McpGateway.Module.Infrastructure.Configuration;

public sealed record CodexBackendSettings(
    string ExecutablePath,
    IReadOnlyList<string> AdditionalArguments,
    string WorkingDirectory)
{
    public static CodexBackendSettings Create(IConfiguration configuration)
    {
        var executablePath = CodexBackendPathResolver.Resolve(configuration["MCP_BACKEND_CODEX_PATH"]);
        var args = CommandLineTokenizer.Tokenize(configuration["MCP_BACKEND_CODEX_ARGS"]);
        var workingDirectory = ResolveWorkingDirectory(configuration["MCP_BACKEND_CODEX_WORKING_DIR"], executablePath);
        return new CodexBackendSettings(executablePath, args, workingDirectory);
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

