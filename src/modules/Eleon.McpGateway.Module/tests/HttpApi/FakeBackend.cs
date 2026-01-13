using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Test.HttpApi;

internal sealed class FakeBackend : IMcpBackend
{
    private readonly List<Channel<JsonNode>> subscribers = new();
    private readonly List<JsonNode> buffered = new();
    private readonly object sync = new();
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
    {
        var channel = Channel.CreateUnbounded<JsonNode>();
        lock (sync)
        {
            subscribers.Add(channel);
            if (buffered.Count > 0)
            {
                foreach (var message in buffered)
                {
                    channel.Writer.TryWrite(message);
                }
                buffered.Clear();
            }
        }

        return channel.Reader.ReadAllAsync(cancellationToken);
    }

    public void Enqueue(JsonNode message)
    {
        List<Channel<JsonNode>> snapshot;
        lock (sync)
        {
            if (subscribers.Count == 0)
            {
                buffered.Add(message);
                return;
            }
            snapshot = subscribers.ToList();
        }

        foreach (var channel in snapshot)
        {
            channel.Writer.TryWrite(message);
        }
    }

    public ValueTask DisposeAsync()
    {
        List<Channel<JsonNode>> snapshot;
        lock (sync)
        {
            snapshot = subscribers.ToList();
            subscribers.Clear();
            buffered.Clear();
        }

        foreach (var channel in snapshot)
        {
            channel.Writer.TryComplete();
        }
        return ValueTask.CompletedTask;
    }
}
