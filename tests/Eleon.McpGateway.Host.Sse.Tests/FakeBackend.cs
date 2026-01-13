using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Eleon.McpGateway.Host.Sse;

namespace Eleon.McpGateway.Host.Sse.Tests;

internal sealed class FakeBackend : IMcpBackend
{
    private readonly Channel<JsonNode> outbound = Channel.CreateUnbounded<JsonNode>();
    private readonly List<JsonNode> sent = new();

    public FakeBackend(string name = "fake")
    {
        Name = name;
    }

    public string Name { get; }

    public IReadOnlyList<JsonNode> SentMessages => sent.AsReadOnly();

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SendAsync(JsonNode message, CancellationToken cancellationToken)
    {
        sent.Add(message);
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<JsonNode> ReceiveAsync(CancellationToken cancellationToken)
        => outbound.Reader.ReadAllAsync(cancellationToken);

    public void Enqueue(JsonNode message)
    {
        outbound.Writer.TryWrite(message);
    }

    public ValueTask DisposeAsync()
    {
        outbound.Writer.TryComplete();
        return ValueTask.CompletedTask;
    }
}
