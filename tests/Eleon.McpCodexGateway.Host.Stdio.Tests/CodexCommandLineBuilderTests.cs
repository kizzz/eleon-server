using Eleon.McpCodexGateway.Host.Stdio;
using FluentAssertions;

namespace Eleon.McpCodexGateway.Host.Stdio.Tests;

public sealed class CodexCommandLineBuilderTests
{
    [Fact]
    public void BuildArguments_AddsWorkspaceSandboxAndExtras()
    {
        var options = new CodexProcessOptions("/workspace", "workspace-write", new[] { "--feature", "alpha" });

        var arguments = CodexCommandLineBuilder.BuildArguments(options);

        arguments.Should().ContainInOrder("--cd", "/workspace", "--sandbox", "workspace-write", "mcp-server", "--feature", "alpha");
    }

    [Fact]
    public void CreateStartInfo_SetsRedirectsUtf8AndWorkingDirectory()
    {
        var options = new CodexProcessOptions("C:/repo/workspace", "workspace-write", Array.Empty<string>());

        var startInfo = CodexCommandLineBuilder.CreateStartInfo(options);

        startInfo.RedirectStandardInput.Should().BeTrue();
        startInfo.RedirectStandardOutput.Should().BeTrue();
        startInfo.RedirectStandardError.Should().BeTrue();
        startInfo.UseShellExecute.Should().BeFalse();
        startInfo.CreateNoWindow.Should().BeTrue();
        startInfo.StandardInputEncoding!.EncodingName.Should().Contain("UTF-8");
        startInfo.WorkingDirectory.Should().Be(options.WorkspaceDirectory);
        startInfo.ArgumentList.Should().ContainInOrder("--cd", options.WorkspaceDirectory, "--sandbox", options.SandboxMode, "mcp-server");
    }

    [Fact]
    public void FormatForLogging_QuotesArgumentsWithWhitespace()
    {
        var options = new CodexProcessOptions("/workspace", "workspace-write", new[] { "--auth-session", "last token" });
        var startInfo = CodexCommandLineBuilder.CreateStartInfo(options);

        var formatted = CodexCommandLineBuilder.FormatForLogging(startInfo);

        formatted.Should().Contain("codex --cd /workspace --sandbox workspace-write mcp-server --auth-session \"last token\"");
    }
}
