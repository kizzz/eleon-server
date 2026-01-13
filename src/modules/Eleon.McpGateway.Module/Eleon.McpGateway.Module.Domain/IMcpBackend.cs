using System.Text.Json.Nodes;

namespace Eleon.McpGateway.Module.Domain;

public interface IMcpBackend : IAsyncDisposable
{
    string Name { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task SendAsync(JsonNode message, CancellationToken cancellationToken);

    IAsyncEnumerable<JsonNode> ReceiveAsync(CancellationToken cancellationToken);
}

