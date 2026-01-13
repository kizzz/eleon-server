using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;

/// <summary>
/// Fake backend with channel-based communication for concurrency tests.
/// Has inbound channel (requests received via SendAsync) and outbound channel (responses/notifications via ReceiveAsync).
/// </summary>
internal sealed class FakeBackendForConcurrency : IMcpBackend
{
    private readonly Channel<JsonNode> inbound = Channel.CreateUnbounded<JsonNode>();
    private readonly Channel<JsonNode> outbound = Channel.CreateUnbounded<JsonNode>();
    private readonly List<JsonNode> sent = new();

    public FakeBackendForConcurrency(string name = "fake")
    {
        Name = name;
    }

    public string Name { get; }

    public IReadOnlyList<JsonNode> SentMessages => sent.AsReadOnly();

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SendAsync(JsonNode message, CancellationToken cancellationToken)
    {
        sent.Add(message);
        inbound.Writer.TryWrite(message);
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<JsonNode> ReceiveAsync(CancellationToken cancellationToken)
        => outbound.Reader.ReadAllAsync(cancellationToken);

    /// <summary>
    /// Enqueue a response/notification to be received via ReceiveAsync.
    /// </summary>
    public void Enqueue(JsonNode message)
    {
        outbound.Writer.TryWrite(message);
    }

    /// <summary>
    /// Try to read a request that was sent via SendAsync.
    /// </summary>
    public async Task<JsonNode?> TryReadInboundAsync(CancellationToken cancellationToken)
    {
        if (await inbound.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
        {
            if (inbound.Reader.TryRead(out var message))
            {
                return message;
            }
        }
        return null;
    }

    public ValueTask DisposeAsync()
    {
        inbound.Writer.TryComplete();
        outbound.Writer.TryComplete();
        return ValueTask.CompletedTask;
    }
}
