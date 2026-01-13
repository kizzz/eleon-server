using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Test.TestBase;
using Eleon.McpGateway.Module.Test.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Eleon.McpGateway.Module.Test.HttpApi;

[Trait("Category", "Manual")]
public sealed class GatewayEndpointsTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new();
    private readonly FakeBackend secondaryBackend = new("secondary");

    public GatewayEndpointsTests()
    {
        RegisterBackends(backend, secondaryBackend);
    }

    [ManualTestFact]
    public async Task Post_InvalidJson_ReturnsBadRequest()
    {
        // HttpClient is already available from base class();
        var response = await HttpClient.PostAsync("/sse", new StringContent("not-json", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [ManualTestFact]
    public async Task Post_MissingPayload_ReturnsBadRequest()
    {
        // HttpClient is already available from base class();
        var response = await HttpClient.PostAsync("/sse", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [ManualTestFact]
    public async Task Post_ClonesPayloadBeforeForwarding()
    {
        // HttpClient is already available from base class();
        var payload = JsonNode.Parse("""{"id":"abc","method":"do"}""")!;

        var response = await HttpClient.PostAsync("/sse", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        payload["id"] = "mutated";

        backend.SentMessages.Should().ContainSingle();
        backend.SentMessages.Single()["id"]!.GetValue<string>().Should().Be("abc");
    }

    [ManualTestFact]
    public async Task Sse_AllowsConcurrentConnectionsAcrossDifferentBackends()
    {
        var first = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        var second = await HttpClient.GetAsync("/sse/secondary", HttpCompletionOption.ResponseHeadersRead);
        var third = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        second.StatusCode.Should().Be(HttpStatusCode.OK);
        third.StatusCode.Should().Be(HttpStatusCode.OK);

        first.Dispose();
        second.Dispose();
        third.Dispose();
    }

    [ManualTestFact]
    public async Task Sse_ReturnsNotFoundForUnknownBackend()
    {

        var response = await HttpClient.GetAsync("/sse/does-not-exist", HttpCompletionOption.ResponseHeadersRead);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [ManualTestFact]
    public async Task Post_ReturnsNotFoundForUnknownBackend()
    {
        // HttpClient is already available from base class();
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"initialize"}""")!;

        var response = await HttpClient.PostAsync("/sse/does-not-exist", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [ManualTestFact]
    public async Task Sse_FormatsEventsWithEventAndDataLines()
    {

        using var sseResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
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

}

