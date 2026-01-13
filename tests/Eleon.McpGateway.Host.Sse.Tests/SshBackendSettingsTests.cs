using Eleon.McpGateway.Host.Sse;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class SshBackendSettingsTests
{
    [Fact]
    public void Create_UsesConfiguredValuesAndTokenizesArgs()
    {
        using var tempExecutable = new TempFile();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MCP_BACKEND_SSH_PATH"] = tempExecutable.Path,
                ["MCP_BACKEND_SSH_ARGS"] = "--foo bar",
                ["MCP_BACKEND_SSH_WORKING_DIR"] = Path.GetDirectoryName(tempExecutable.Path)
            })
            .Build();

        var settings = SshBackendSettings.Create(config);

        settings.ExecutablePath.Should().Be(Path.GetFullPath(tempExecutable.Path));
        settings.AdditionalArguments.Should().BeEquivalentTo(new[] { "--foo", "bar" }, o => o.WithStrictOrdering());
        settings.WorkingDirectory.Should().Be(Path.GetDirectoryName(tempExecutable.Path));
    }

    [Fact]
    public void Create_FallsBackToExecutableDirectoryWhenWorkingDirMissing()
    {
        using var tempExecutable = new TempFile();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MCP_BACKEND_SSH_PATH"] = tempExecutable.Path
            })
            .Build();

        var settings = SshBackendSettings.Create(config);

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
