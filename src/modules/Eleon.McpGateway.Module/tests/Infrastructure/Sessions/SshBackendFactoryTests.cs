using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Backends;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Sessions;

public sealed class SshBackendFactoryTests
{
    [Fact]
    public async Task CreateAsync_CreatesNewInstance()
    {
        var settings = new SshBackendSettings("test.exe", Array.Empty<string>(), "/tmp");
        var factory = new SshBackendFactory(settings, NullLogger<SshMcpBackend>.Instance);

        var backend1 = await factory.CreateAsync("ssh-stdio", CancellationToken.None);
        var backend2 = await factory.CreateAsync("ssh-stdio", CancellationToken.None);

        backend1.Should().NotBeSameAs(backend2);
        backend1.Name.Should().Be("ssh-stdio");
        backend2.Name.Should().Be("ssh-stdio");
    }
}

