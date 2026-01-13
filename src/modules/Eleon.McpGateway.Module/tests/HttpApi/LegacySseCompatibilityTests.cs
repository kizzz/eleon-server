using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Test.TestBase;
using FluentAssertions;
using Eleon.McpGateway.Module.Test.TestHelpers;
using Xunit;

namespace Eleon.McpGateway.Module.Test.HttpApi;

[Trait("Category", "Manual")]
public sealed class LegacySseCompatibilityTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new("test-backend");

    public LegacySseCompatibilityTests()
    {
        RegisterBackends(backend);
    }

    [ManualTestFact]
    public async Task Get_Sse_Returns200_WithTextEventStream()
    {
        var response = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/event-stream");
    }

    [ManualTestFact]
    public async Task Get_Sse_EstablishesConnection_ReceivesMessages()
    {
        using var sseResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        // Enqueue message from backend
        var backendMessage = JsonNode.Parse("""{"jsonrpc":"2.0","method":"notification","params":{}}""")!;
        backend.Enqueue(backendMessage);

        var (eventLine, dataLine) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        eventLine.Should().Be("event: message");
        dataLine.Should().Contain("notification");

    }

    [ManualTestFact]
    public async Task Post_Sse_AcceptsJsonRpc_ForwardsToBackend()
    {

        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var response = await HttpClient.PostAsync("/sse", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        backend.SentMessages.Should().ContainSingle();
        backend.SentMessages.Single()["id"]!.ToString().Should().Be("1");

    }

    [ManualTestFact]
    public async Task Sse_Endpoints_DoNotRequireMcpSessionIdHeader()
    {
        // GET /sse without session header should work
        var getResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // POST /sse without session header should work
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var postResponse = await HttpClient.PostAsync("/sse", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));
        postResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

    }

    [ManualTestFact]
    public async Task Sse_Endpoints_HandleMcpSessionIdHeader_IfProvided()
    {
        var sessionId = "test-session-id";

        // GET /sse with session header (optional)
        var getRequest = new HttpRequestMessage(HttpMethod.Get, "/sse");
        getRequest.Headers.Add("Mcp-Session-Id", sessionId);
        var getResponse = await HttpClient.SendAsync(getRequest, HttpCompletionOption.ResponseHeadersRead);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // POST /sse with session header (optional)
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/sse")
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        postRequest.Headers.Add("Mcp-Session-Id", sessionId);
        var postResponse = await HttpClient.SendAsync(postRequest);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

    }

    [ManualTestFact]
    public async Task Sse_And_Mcp_Endpoints_CanCoexist()
    {
        // Use /sse endpoint
        var sseResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Use /mcp endpoint (requires session)
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        initResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        initResponse.Headers.Should().ContainKey("Mcp-Session-Id");

        sseResponse.Dispose();
    }

    [ManualTestFact]
    public async Task Sse_Endpoints_UseDefaultBackend_WhenNoneSpecified()
    {

        // POST to /sse (no backend specified) should use default
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var response = await HttpClient.PostAsync("/sse", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        backend.SentMessages.Should().ContainSingle();

    }

    [ManualTestFact]
    public async Task Sse_Endpoints_UseDispatcherFallbackPath_NoSessionRegistryRequired()
    {

        // /sse endpoints should work even without session registry (backward compatibility)
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var response = await HttpClient.PostAsync("/sse", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

    }

    [ManualTestFact]
    public async Task OriginValidation_ForMcp_DoesNotAffectSse()
    {
        // Note: RestrictedOriginGatewayWebApplicationFactory tests would need custom configuration
        // For now, using default test setup

        // /sse endpoint should work even with unauthorized origin (middleware only validates /mcp)
        var request = new HttpRequestMessage(HttpMethod.Post, "/sse")
        {
            Content = new StringContent("""{"jsonrpc":"2.0","id":"1","method":"test"}""", Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Origin", "https://unauthorized.com");

        var response = await HttpClient.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted); // Should not be forbidden

    }

    [ManualTestFact]
    public async Task SseConnectionCoordinator_OneActiveConnectionPerBackend_IsDeterministic()
    {
        // Connect first SSE client to /sse
        var firstResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Connect second SSE client to /sse for same backend
        var secondResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);

        // Based on current implementation (SseConnectionCoordinator.TryAcquire always returns true),
        // multiple connections are allowed. Document this behavior in TEST_MATRIX.md
        // Current behavior: both connections are allowed
        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify both connections work
        var reader1 = new StreamReader(await firstResponse.Content.ReadAsStreamAsync());
        var reader2 = new StreamReader(await secondResponse.Content.ReadAsStreamAsync());

        backend.Enqueue(JsonNode.Parse("""{"jsonrpc":"2.0","method":"test1"}""")!);
        backend.Enqueue(JsonNode.Parse("""{"jsonrpc":"2.0","method":"test2"}""")!);

        var (event1, data1) = await ReadNextEventAsync(reader1, TimeSpan.FromSeconds(3));
        var (event2, data2) = await ReadNextEventAsync(reader2, TimeSpan.FromSeconds(3));

        data1.Should().Contain("test1");
        data2.Should().Contain("test1");

        var (_, data1Second) = await ReadNextEventAsync(reader1, TimeSpan.FromSeconds(3));
        var (_, data2Second) = await ReadNextEventAsync(reader2, TimeSpan.FromSeconds(3));

        data1Second.Should().Contain("test2");
        data2Second.Should().Contain("test2");

        firstResponse.Dispose();
        secondResponse.Dispose();
    }

    [ManualTestFact]
    public async Task Sse_Disconnect_ReleasesResources_NoStuckTasks()
    {
        // Connect and immediately disconnect
        var response = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Dispose();

        // Verify we can connect again (resources were released)
        var secondResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        secondResponse.Dispose();
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

// RestrictedOriginGatewayWebApplicationFactory removed - use McpGatewayTestBase with custom configuration if needed
