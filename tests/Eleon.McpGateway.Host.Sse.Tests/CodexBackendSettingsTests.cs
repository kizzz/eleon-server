using Eleon.McpGateway.Host.Sse;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class CodexBackendSettingsTests
{
    [Fact]
    public void Create_UsesConfiguredValuesAndTokenizesArgs()
    {
        using var tempExecutable = new TempFile();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MCP_BACKEND_CODEX_PATH"] = tempExecutable.Path,
                ["MCP_BACKEND_CODEX_ARGS"] = "--auth-session last --no-key",
                ["MCP_BACKEND_CODEX_WORKING_DIR"] = Path.GetDirectoryName(tempExecutable.Path)
            })
            .Build();

        var settings = CodexBackendSettings.Create(config);

        settings.ExecutablePath.Should().Be(Path.GetFullPath(tempExecutable.Path));
        settings.AdditionalArguments.Should().BeEquivalentTo(new[] { "--auth-session", "last", "--no-key" }, o => o.WithStrictOrdering());
        settings.WorkingDirectory.Should().Be(Path.GetDirectoryName(tempExecutable.Path));
    }

    [Fact]
    public void Create_FallsBackToExecutableDirectoryWhenWorkingDirMissing()
    {
        using var tempExecutable = new TempFile();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MCP_BACKEND_CODEX_PATH"] = tempExecutable.Path
            })
            .Build();

        var settings = CodexBackendSettings.Create(config);

        settings.WorkingDirectory.Should().Be(Path.GetDirectoryName(tempExecutable.Path));
    }

    private sealed class TempFile : IDisposable
    {
        public string Path { get; }
        public TempFile()
        {
            Path = System.IO.Path.GetTempFileName();
            File.WriteAllText(Path, string.Empty);
        }

        public void Dispose()
        {
            try
            {
                File.Delete(Path);
            }
            catch
            {
                // ignore
            }
        }
    }
}
