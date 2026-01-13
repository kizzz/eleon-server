using Eleon.McpGateway.Module.Infrastructure.PathResolvers;
using FluentAssertions;

namespace Eleon.McpGateway.Module.Test.Infrastructure.PathResolvers;

public sealed class CodexBackendPathResolverTests
{
    [Fact]
    public void Resolve_ReturnsConfiguredAbsolutePath()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            CodexBackendPathResolver.Resolve(tempFile).Should().Be(Path.GetFullPath(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Resolve_ExpandsEnvironmentVariables()
    {
        var tempDirectory = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar);
        Environment.SetEnvironmentVariable("CODEX_TEMP_DIR", tempDirectory);
        var configured = "%CODEX_TEMP_DIR%/codex.dll";

        var resolved = CodexBackendPathResolver.Resolve(configured);

        resolved.Should().StartWith(tempDirectory);
    }
}

