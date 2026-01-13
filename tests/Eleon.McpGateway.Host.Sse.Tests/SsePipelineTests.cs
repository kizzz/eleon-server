using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class SsePipelineTests : IAsyncDisposable
{
    private readonly FakeBackend backend = new();
    private readonly GatewayWebApplicationFactory factory;

    public SsePipelineTests()
    {
        factory = new GatewayWebApplicationFactory(backend);
    }

    [Fact]
    public async Task Gateway_ForwardsPostAndStreamsBackendResponses()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var sseResponse = await client.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        var requestPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"echo"}""")!;
        var postResponse = await client.PostAsync("/sse", new StringContent(requestPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        postResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        backend.SentMessages
            .Where(node => node["method"] != null && node["method"]!.GetValue<string>() == "echo")
            .Should()
            .ContainSingle();

        var backendReply = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","result":{"ok":true}}""")!;
        backend.Enqueue(backendReply);

        var data = await ReadNextSseEventAsync(reader, TimeSpan.FromSeconds(3));
        data.Should().Be(backendReply.ToJsonString());
    }

    private static async Task<string> ReadNextSseEventAsync(StreamReader reader, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        var builder = new StringBuilder();
        while (!cts.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
            {
                break;
            }

            if (line.StartsWith("data: ", StringComparison.Ordinal))
            {
                builder.Append(line[6..]);
            }
            else if (line.Length == 0 && builder.Length > 0)
            {
                return builder.ToString();
            }
        }

        throw new TimeoutException("Timed out waiting for SSE message.");
    }

    public async ValueTask DisposeAsync()
    {
        await factory.DisposeAsync();
        await backend.DisposeAsync();
    }
}
