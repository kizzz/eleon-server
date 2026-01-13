using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class GatewayEndpointsTests : IAsyncDisposable
{
    private readonly FakeBackend backend = new();
    private readonly FakeBackend secondaryBackend = new("secondary");
    private readonly GatewayWebApplicationFactory factory;

    public GatewayEndpointsTests()
    {
        factory = new GatewayWebApplicationFactory(backend, secondaryBackend);
    }

    [Fact]
    public async Task Post_InvalidJson_ReturnsBadRequest()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsync("/sse", new StringContent("not-json", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_MissingPayload_ReturnsBadRequest()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsync("/sse", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ClonesPayloadBeforeForwarding()
    {
        var client = factory.CreateClient();
        var payload = JsonNode.Parse("""{"id":"abc","method":"do"}""")!;

        var response = await client.PostAsync("/sse", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        payload["id"] = "mutated";

        backend.SentMessages.Should().ContainSingle();
        backend.SentMessages.Single()["id"]!.GetValue<string>().Should().Be("abc");
    }

    [Fact]
    public async Task Sse_AllowsConcurrentConnectionsAcrossDifferentBackends()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var first = await client.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        var second = await client.GetAsync("/sse/secondary", HttpCompletionOption.ResponseHeadersRead);
        var third = await client.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        second.StatusCode.Should().Be(HttpStatusCode.OK);
        third.StatusCode.Should().Be(HttpStatusCode.OK);

        first.Dispose();
        second.Dispose();
        third.Dispose();
    }

    [Fact]
    public async Task Sse_ReturnsNotFoundForUnknownBackend()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/sse/does-not-exist", HttpCompletionOption.ResponseHeadersRead);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ReturnsNotFoundForUnknownBackend()
    {
        var client = factory.CreateClient();
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"initialize"}""")!;

        var response = await client.PostAsync("/sse/does-not-exist", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Sse_FormatsEventsWithEventAndDataLines()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var sseResponse = await client.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        var backendReply = JsonNode.Parse("""{"jsonrpc":"2.0","id":"42","result":{"ok":true}}""")!;
        backend.Enqueue(backendReply);

        var (eventLine, dataLine) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        eventLine.Should().Be("event: message");
        dataLine.Should().Be($"data: {backendReply.ToJsonString()}");
    }

    private static async Task<(string EventLine, string DataLine)> ReadNextEventAsync(StreamReader reader, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        string? eventLine = null;
        string? dataLine = null;
        while (!cts.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
            {
                break;
            }

            if (line.StartsWith("event:", StringComparison.Ordinal))
            {
                eventLine = line;
            }
            else if (line.StartsWith("data:", StringComparison.Ordinal))
            {
                dataLine = line;
            }
            else if (line.Length == 0 && eventLine != null && dataLine != null)
            {
                return (eventLine, dataLine);
            }
        }

        throw new TimeoutException("No SSE event received in allotted time.");
    }

    public async ValueTask DisposeAsync()
    {
        await factory.DisposeAsync();
        await backend.DisposeAsync();
        await secondaryBackend.DisposeAsync();
    }
}
