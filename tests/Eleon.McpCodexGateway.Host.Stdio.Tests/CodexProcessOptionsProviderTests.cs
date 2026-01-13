using Eleon.McpCodexGateway.Host.Stdio;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.McpCodexGateway.Host.Stdio.Tests;

public sealed class CodexProcessOptionsProviderTests
{
    [Fact]
    public void GetOptions_UsesEnvironmentConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CODEX_WORKSPACE_DIR"] = "/tmp/workspace",
                ["CODEX_SANDBOX_MODE"] = "isolated",
                ["CODEX_EXTRA_ARGS"] = "--log-level trace \"quoted arg\""
            })
            .Build();

        var provider = new CodexProcessOptionsProvider(configuration, NullLogger<CodexProcessOptionsProvider>.Instance);

        var options = provider.GetOptions();

        options.WorkspaceDirectory.Replace('\\', '/').Should().EndWith("tmp/workspace");
        options.SandboxMode.Should().Be("isolated");
        options.ExtraArguments.Should().BeEquivalentTo(new[] { "--log-level", "trace", "quoted arg" }, options => options.WithStrictOrdering());
    }

    [Fact]
    public void GetOptions_FallsBackToDefaultsWhenConfigMissingOrWhitespace()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CODEX_WORKSPACE_DIR"] = "   ",
                ["CODEX_SANDBOX_MODE"] = "",
                ["CODEX_EXTRA_ARGS"] = null
            })
            .Build();

        var provider = new CodexProcessOptionsProvider(configuration, NullLogger<CodexProcessOptionsProvider>.Instance);

        var options = provider.GetOptions();

        options.WorkspaceDirectory.Should().EndWith(Path.Combine("workspace"));
        options.SandboxMode.Should().Be("workspace-write");
        options.ExtraArguments.Should().BeEmpty();
    }

    [Fact]
    public void GetOptions_ParsesAuthSessionWithoutKey()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CODEX_EXTRA_ARGS"] = "--auth-session last --no-key"
            })
            .Build();

        var provider = new CodexProcessOptionsProvider(configuration, NullLogger<CodexProcessOptionsProvider>.Instance);

        var options = provider.GetOptions();

        options.ExtraArguments.Should().BeEquivalentTo(new[] { "--auth-session", "last", "--no-key" }, opt => opt.WithStrictOrdering());
    }
}
