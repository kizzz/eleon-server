using System;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Infrastructure.Backends;
using Eleon.McpGateway.Module.Domain;
using FluentAssertions;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Backends;

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

    private sealed class FakeBackend : IMcpBackend
    {
        public string Name => "fake";

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SendAsync(JsonNode message, CancellationToken cancellationToken) => Task.CompletedTask;

        public IAsyncEnumerable<JsonNode> ReceiveAsync(CancellationToken cancellationToken)
        {
            return Empty();

            static async IAsyncEnumerable<JsonNode> Empty()
            {
                await Task.CompletedTask;
                yield break;
            }
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

