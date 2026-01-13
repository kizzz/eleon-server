using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Host.Sse;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class McpGatewayDispatcherTests
{
    [Fact]
    public async Task ForwardAsync_ClonesPayloadBeforeSend()
    {
        var backend = new RecordingBackend();
        var dispatcher = new McpGatewayDispatcher(
            new SingleBackendRegistry(backend),
            NullLogger<McpGatewayDispatcher>.Instance);
        var payload = JsonNode.Parse("""{"id":1,"method":"echo"}""")!;

        await dispatcher.ForwardAsync(payload, backend.Name, CancellationToken.None);
        payload["id"] = 99;

        backend.Sent.Should().ContainSingle();
        backend.Sent[0]!["id"]!.GetValue<int>().Should().Be(1);
    }

    [Fact]
    public void ResolveBackendNameOrDefault_ReturnsDefaultWhenNullOrEmpty()
    {
        var backend = new RecordingBackend();
        var dispatcher = new McpGatewayDispatcher(
            new SingleBackendRegistry(backend),
            NullLogger<McpGatewayDispatcher>.Instance);

        dispatcher.ResolveBackendNameOrDefault(null).Should().Be(backend.Name);
        dispatcher.ResolveBackendNameOrDefault(string.Empty).Should().Be(backend.Name);
    }

    [Fact]
    public void EnsureBackendExists_ThrowsForUnknown()
    {
        var backend = new RecordingBackend();
        var dispatcher = new McpGatewayDispatcher(
            new SingleBackendRegistry(backend),
            NullLogger<McpGatewayDispatcher>.Instance);

        Action act = () => dispatcher.EnsureBackendExists("missing");
        act.Should().Throw<KeyNotFoundException>();
    }

    private sealed class SingleBackendRegistry : IMcpBackendRegistry
    {
        private readonly IMcpBackend backend;
        public SingleBackendRegistry(IMcpBackend backend) => this.backend = backend;
        public IMcpBackend GetBackend(string name)
        {
            if (!string.Equals(name, backend.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new KeyNotFoundException(name);
            }

            return backend;
        }
        public IMcpBackend GetDefaultBackend() => backend;
        public IReadOnlyCollection<IMcpBackend> GetAll() => new[] { backend };
    }

    private sealed class RecordingBackend : IMcpBackend
    {
        public string Name => "recording";
        public List<JsonNode> Sent { get; } = new();

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SendAsync(JsonNode message, CancellationToken cancellationToken)
        {
            Sent.Add(message.DeepClone() ?? message);
            return Task.CompletedTask;
        }

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
