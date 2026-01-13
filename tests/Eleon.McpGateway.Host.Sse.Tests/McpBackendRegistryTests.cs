using System;
using Eleon.McpGateway.Host.Sse;
using FluentAssertions;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class McpBackendRegistryTests
{
    [Fact]
    public void Constructor_ThrowsWhenNoBackends()
    {
        Action act = () => new McpBackendRegistry(Array.Empty<IMcpBackend>());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetBackend_ReturnsRegisteredInstance()
    {
        var backend = new FakeBackend();
        var registry = new McpBackendRegistry(new[] { backend });

        registry.GetBackend(backend.Name).Should().BeSameAs(backend);
        registry.GetDefaultBackend().Should().BeSameAs(backend);
        registry.GetAll().Should().ContainSingle().And.Contain(backend);
    }

    [Fact]
    public void GetBackend_ThrowsForUnknownName()
    {
        var backend = new FakeBackend();
        var registry = new McpBackendRegistry(new[] { backend });

        Action act = () => registry.GetBackend("missing");

        act.Should().Throw<KeyNotFoundException>();
    }
}
