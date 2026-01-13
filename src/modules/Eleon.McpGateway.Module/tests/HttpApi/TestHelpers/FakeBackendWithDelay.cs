using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;

/// <summary>
/// Fake backend that delays responses for timeout testing.
/// </summary>
internal sealed class FakeBackendWithDelay : IMcpBackend
{
    private readonly Channel<JsonNode> outbound = Channel.CreateUnbounded<JsonNode>();
    private readonly List<JsonNode> sent = new();
    private readonly TimeSpan delay;

    public FakeBackendWithDelay(string name = "fake", TimeSpan? delay = null)
    {
        Name = name;
        this.delay = delay ?? TimeSpan.FromSeconds(5);
    }

    public string Name { get; }

    public IReadOnlyList<JsonNode> SentMessages => sent.AsReadOnly();

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SendAsync(JsonNode message, CancellationToken cancellationToken)
    {
        sent.Add(message);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<JsonNode> ReceiveAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var message in outbound.Reader.ReadAllAsync(cancellationToken).WithCancellation(cancellationToken))
        {
            await Task.Delay(delay, cancellationToken);
            yield return message;
        }
    }

    /// <summary>
    /// Enqueue a response that will be delayed before being returned.
    /// </summary>
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
