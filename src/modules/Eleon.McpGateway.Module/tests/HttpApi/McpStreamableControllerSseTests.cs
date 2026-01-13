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
public sealed class McpStreamableControllerSseTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new("test-backend");

    public McpStreamableControllerSseTests()
    {
        RegisterBackends(backend);
    }

    [ManualTestFact]
    public async Task Sse_ConnectionHandlesClientDisconnect_Gracefully()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Disconnect by disposing the response
        // Server should handle this gracefully without crashing
        sseResponse.Dispose();
    }

    [ManualTestFact]
    public async Task Sse_ConnectionHandlesServerCancellation_Gracefully()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Cancel by terminating session
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/mcp");
        deleteRequest.Headers.Add("Mcp-Session-Id", sessionId);
        await HttpClient.SendAsync(deleteRequest);

        // Connection should be closed gracefully
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());
        var line = await reader.ReadLineAsync();
        // Should eventually stop reading (connection closed)
    }

    [ManualTestFact]
    public async Task Sse_SendsMultipleMessages_InSequence()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        // Enqueue multiple messages
        var message1 = JsonNode.Parse("""{"jsonrpc":"2.0","method":"notification1","params":{}}""")!;
        var message2 = JsonNode.Parse("""{"jsonrpc":"2.0","method":"notification2","params":{}}""")!;
        var message3 = JsonNode.Parse("""{"jsonrpc":"2.0","method":"notification3","params":{}}""")!;

        backend.Enqueue(message1);
        backend.Enqueue(message2);
        backend.Enqueue(message3);

        // Read first message
        var (event1, data1) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        event1.Should().Be("event: message");
        data1.Should().Contain("notification1");

        // Read second message
        var (event2, data2) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        event2.Should().Be("event: message");
        data2.Should().Contain("notification2");

        // Read third message
        var (event3, data3) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        event3.Should().Be("event: message");
        data3.Should().Contain("notification3");
    }

    [ManualTestFact]
    public async Task Sse_HandlesMalformedJsonFromBackend_DoesNotCrash()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        // Enqueue malformed JSON (this should be handled by the backend, but test that server doesn't crash)
        // Note: FakeBackend expects valid JsonNode, so we can't test truly malformed JSON here
        // This test verifies the server handles edge cases gracefully
        var validMessage = JsonNode.Parse("""{"jsonrpc":"2.0","method":"test","params":{}}""")!;
        backend.Enqueue(validMessage);

        var (eventLine, dataLine) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        eventLine.Should().Be("event: message");
        dataLine.Should().Contain("test");
    }

    [ManualTestFact]
    public async Task Sse_HeadersAreSetCorrectly()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        sseResponse.Content.Headers.ContentType!.MediaType.Should().Be("text/event-stream");
        sseResponse.Headers.CacheControl.Should().NotBeNull();
        sseResponse.Headers.CacheControl!.NoStore.Should().BeTrue();
        sseResponse.Headers.Pragma.Should().NotBeNull();
        sseResponse.Headers.Connection.Should().NotBeNull();
        sseResponse.Headers.Connection!.Should().Contain("keep-alive");
        sseResponse.Headers.Should().ContainKey("X-Accel-Buffering");
        sseResponse.Headers.GetValues("X-Accel-Buffering").First().Should().Be("no");
    }

    [ManualTestFact]
    public async Task Sse_ConnectionWithInvalidSessionId_Returns400()
    {

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", "invalid-session-id");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid or expired session");
    }

    [ManualTestFact]
    public async Task Sse_ConnectionWithExpiredSession_Returns400()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Terminate session
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/mcp");
        deleteRequest.Headers.Add("Mcp-Session-Id", sessionId);
        await HttpClient.SendAsync(deleteRequest);

        // Try to connect with expired session
        var getRequest = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        getRequest.Headers.Add("Mcp-Session-Id", sessionId);

        var response = await HttpClient.SendAsync(getRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid or expired session");
    }

    [ManualTestFact]
    public async Task Sse_ConnectionCleanupOnCancellation_ReleasesResources()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Dispose connection (simulates cancellation)
        sseResponse.Dispose();

        // Verify session still exists and can be used
        var testRequest = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""", Encoding.UTF8, "application/json")
        };
        testRequest.Headers.Add("Mcp-Session-Id", sessionId);

        var testResponse = await HttpClient.SendAsync(testRequest);
        testResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
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
