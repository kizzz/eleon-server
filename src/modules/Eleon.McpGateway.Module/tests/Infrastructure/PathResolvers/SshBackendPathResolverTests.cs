using Eleon.McpGateway.Module.Infrastructure.PathResolvers;
using FluentAssertions;

namespace Eleon.McpGateway.Module.Test.Infrastructure.PathResolvers;

public sealed class SshBackendPathResolverTests
{
    [Fact]
    public void Resolve_ReturnsConfiguredAbsolutePath()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            SshBackendPathResolver.Resolve(tempFile).Should().Be(Path.GetFullPath(tempFile));
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
        Environment.SetEnvironmentVariable("SSH_TEMP_DIR", tempDirectory);
        var configured = "%SSH_TEMP_DIR%/ssh.dll";

        var resolved = SshBackendPathResolver.Resolve(configured);

        resolved.Should().StartWith(tempDirectory);
    }
}

