using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;

/// <summary>
/// Fake backend that can send malformed JSON or error responses for testing error handling.
/// </summary>
internal sealed class FakeBackendWithErrors : IMcpBackend
{
    private readonly Channel<JsonNode> outbound = Channel.CreateUnbounded<JsonNode>();
    private readonly List<JsonNode> sent = new();
    private readonly bool sendMalformedJson;

    public FakeBackendWithErrors(string name = "fake", bool sendMalformedJson = false)
    {
        Name = name;
        this.sendMalformedJson = sendMalformedJson;
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
        if (sendMalformedJson)
        {
            return ReceiveMalformedAsync(cancellationToken);
        }
        return outbound.Reader.ReadAllAsync(cancellationToken);
    }

    private async IAsyncEnumerable<JsonNode> ReceiveMalformedAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Note: We can't actually send malformed JSON through JsonNode,
        // but we can send error responses
        await foreach (var message in outbound.Reader.ReadAllAsync(cancellationToken).WithCancellation(cancellationToken))
        {
            // Return error response instead
            var errorResponse = JsonNode.Parse($$$"""{"jsonrpc":"2.0","id":{{{message["id"]?.ToString() ?? "null"}}},"error":{"code":-32603,"message":"Internal error"}}""")!;
            yield return errorResponse;
        }
    }

    /// <summary>
    /// Enqueue a response (will be converted to error if sendMalformedJson is true).
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
