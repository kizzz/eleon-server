using System.Diagnostics;
using System.Linq;
using System.Text;
using Eleon.McpCodexGateway.Module.Domain.ValueObjects;

namespace Eleon.McpCodexGateway.Module.Application.Services;

public static class CodexCommandLineBuilder
{
    public static ProcessStartInfo CreateStartInfo(CodexProcessOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var startInfo = new ProcessStartInfo("codex")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardInputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            StandardOutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            StandardErrorEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            WorkingDirectory = options.WorkspaceDirectory
        };

        var args = BuildArguments(options);
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        return startInfo;
    }

    public static IReadOnlyList<string> BuildArguments(CodexProcessOptions options)
    {
        var arguments = new List<string>
        {
            "--cd",
            options.WorkspaceDirectory,
            "--sandbox",
            options.SandboxMode,
            "mcp-server"
        };

        if (options.ExtraArguments.Count > 0)
        {
            arguments.AddRange(options.ExtraArguments);
        }

        return arguments;
    }

    public static string FormatForLogging(ProcessStartInfo startInfo)
    {
        var builder = new StringBuilder();
        builder.Append(startInfo.FileName);
        foreach (var arg in startInfo.ArgumentList)
        {
            builder.Append(' ');
            builder.Append(MaybeQuote(arg));
        }

        return builder.ToString();

        static string MaybeQuote(string arg)
        {
            return arg.Any(char.IsWhiteSpace) ? $"\"{arg}\"" : arg;
        }
    }
}

