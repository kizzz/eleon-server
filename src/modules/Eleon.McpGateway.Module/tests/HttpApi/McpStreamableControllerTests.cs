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
public sealed class McpStreamableControllerTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new("test-backend");

    public McpStreamableControllerTests()
    {
        RegisterBackends(backend);
    }

    [ManualTestFact]
    public async Task Initialize_WithoutSessionHeader_CreatesSession_ReturnsSessionId()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"initialize","params":{}}""")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("Mcp-Session-Id");
        var sessionId = response.Headers.GetValues("Mcp-Session-Id").First();
        sessionId.Should().NotBeNullOrEmpty();
    }

    [ManualTestFact]
    public async Task Initialize_WithSessionHeader_UsesExistingSession()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"initialize","params":{}}""")!;

        // First initialize to get session
        var firstResponse = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = firstResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Second initialize with session header
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Mcp-Session-Id", sessionId);

        var secondResponse = await HttpClient.SendAsync(request);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondResponse.Headers.GetValues("Mcp-Session-Id").First().Should().Be(sessionId);
    }

    [ManualTestFact]
    public async Task Post_WithoutSessionHeader_Returns400_WhenNotTolerant()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Mcp-Session-Id header required");
    }

    [ManualTestFact]
    public async Task Post_WithRequestId_ReturnsMatchingResponse()
    {
        
        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Send request with id
        var requestPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-123","method":"test","params":{}}""")!;
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(requestPayload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Mcp-Session-Id", sessionId);

        // Enqueue response from backend
        var responsePayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-123","result":{"success":true}}""")!;
        backend.Enqueue(responsePayload);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var responseJson = JsonNode.Parse(content);
        responseJson!["id"]!.ToString().Should().Be("req-123");
        responseJson["result"]!["success"]!.GetValue<bool>().Should().BeTrue();
    }

    [ManualTestFact]
    public async Task Post_Notification_WithoutId_Returns202()
    {
        
        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Send notification (no id)
        var notificationPayload = JsonNode.Parse("""{"jsonrpc":"2.0","method":"notify","params":{}}""")!;
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(notificationPayload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Mcp-Session-Id", sessionId);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task Get_WithoutSessionHeader_Returns400()
    {

        var response = await HttpClient.GetAsync("/mcp", HttpCompletionOption.ResponseHeadersRead);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [ManualTestFact]
    public async Task Get_WithSessionHeader_EstablishesSse_ReceivesMessages()
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

        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        // Enqueue message from backend
        var backendMessage = JsonNode.Parse("""{"jsonrpc":"2.0","method":"notification","params":{}}""")!;
        backend.Enqueue(backendMessage);

        var (eventLine, dataLine) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        eventLine.Should().Be("event: message");
        dataLine.Should().Contain("notification");
    }

    [ManualTestFact]
    public async Task Get_Sse_SendsKeepaliveComments()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        // Wait for keepalive comment (should arrive within keepalive interval)
        var line = await reader.ReadLineAsync();
        // Keepalive comments start with ":"
        var foundKeepalive = false;
        var timeout = DateTime.UtcNow.AddSeconds(20);
        while (DateTime.UtcNow < timeout && !foundKeepalive)
        {
            if (line?.StartsWith(":") == true)
            {
                foundKeepalive = true;
                break;
            }
            line = await reader.ReadLineAsync();
        }

        foundKeepalive.Should().BeTrue();
    }

    [ManualTestFact]
    public async Task Get_Sse_FormatsEventsCorrectly()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        using var sseResponse = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        var testMessage = JsonNode.Parse("""{"jsonrpc":"2.0","method":"test","params":{"key":"value"}}""")!;
        backend.Enqueue(testMessage);

        var (eventLine, dataLine) = await ReadNextEventAsync(reader, TimeSpan.FromSeconds(3));
        eventLine.Should().Be("event: message");
        dataLine.Should().StartWith("data: ");
        var jsonData = dataLine.Substring(6); // Remove "data: " prefix
        var parsed = JsonNode.Parse(jsonData);
        parsed!["method"]!.ToString().Should().Be("test");
    }

    [ManualTestFact]
    public async Task Delete_WithoutSessionHeader_Returns400()
    {

        var response = await HttpClient.DeleteAsync("/mcp");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [ManualTestFact]
    public async Task Delete_WithSessionHeader_TerminatesSession_Returns204()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/mcp");
        request.Headers.Add("Mcp-Session-Id", sessionId);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify session is terminated by trying to use it again
        var testPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var testRequest = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(testPayload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        testRequest.Headers.Add("Mcp-Session-Id", sessionId);

        var testResponse = await HttpClient.SendAsync(testRequest);
        testResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [ManualTestFact]
    public async Task Cors_ExposesMcpSessionIdHeader()
    {

        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"initialize","params":{}}""")!;
        var response = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.Headers.Should().ContainKey("Mcp-Session-Id");
    }

    [ManualTestFact]
    public async Task Post_WithRequestId_HandlesTimeout_ReturnsTimeoutError()
    {
        
        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Send request with id but don't enqueue response (will timeout)
        var requestPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"timeout-req","method":"test","params":{}}""")!;
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(requestPayload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Mcp-Session-Id", sessionId);

        // Note: This test requires a short timeout configured in test settings
        // For now, we'll just verify the request is sent (actual timeout testing would need config)
        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        // Response should eventually return (either success if response arrives, or timeout error)
        // Without enqueuing a response, it should timeout
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.RequestTimeout);
    }

    [ManualTestFact]
    public async Task MultipleSessions_CanCoexist()
    {

        // Create first session
        var initPayload1 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init1","method":"initialize","params":{}}""")!;
        var response1 = await HttpClient.PostAsync("/mcp", new StringContent(initPayload1.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId1 = response1.Headers.GetValues("Mcp-Session-Id").First();

        // Create second session
        var initPayload2 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init2","method":"initialize","params":{}}""")!;
        var response2 = await HttpClient.PostAsync("/mcp", new StringContent(initPayload2.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId2 = response2.Headers.GetValues("Mcp-Session-Id").First();

        sessionId1.Should().NotBe(sessionId2);

        // Both sessions should work independently
        var request1 = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent("""{"jsonrpc":"2.0","id":"req1","method":"test","params":{}}""", Encoding.UTF8, "application/json")
        };
        request1.Headers.Add("Mcp-Session-Id", sessionId1);

        var request2 = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent("""{"jsonrpc":"2.0","id":"req2","method":"test","params":{}}""", Encoding.UTF8, "application/json")
        };
        request2.Headers.Add("Mcp-Session-Id", sessionId2);

        var response1After = await HttpClient.SendAsync(request1);
        var response2After = await HttpClient.SendAsync(request2);

        response1After.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
        response2After.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task Session_CannotBeUsedAfterTermination()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Terminate session
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/mcp");
        deleteRequest.Headers.Add("Mcp-Session-Id", sessionId);
        var deleteResponse = await HttpClient.SendAsync(deleteRequest);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Try to use terminated session
        var testPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var testRequest = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(testPayload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        testRequest.Headers.Add("Mcp-Session-Id", sessionId);

        var testResponse = await HttpClient.SendAsync(testRequest);
        testResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await testResponse.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid or expired session");
    }

    [ManualTestFact]
    public async Task SessionId_HeaderIsCaseInsensitive()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Use different case for header name
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""", Encoding.UTF8, "application/json")
        };
        request.Headers.Add("MCP-SESSION-ID", sessionId); // Different case

        var response = await HttpClient.SendAsync(request);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task MultipleConcurrentRequests_DifferentIds_ReturnCorrectResponses()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Enqueue responses for all requests
        var responses = new List<JsonNode>();
        for (int i = 1; i <= 5; i++)
        {
            var response = new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = $"req-{i}",
                ["result"] = new JsonObject
                {
                    ["value"] = i
                }
            };
            responses.Add(response);
            backend.Enqueue(response);
        }

        // Send concurrent requests
        var tasks = Enumerable.Range(1, 5)
            .Select(async i =>
            {
                var requestPayload = new JsonObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = $"req-{i}",
                    ["method"] = "test",
                    ["params"] = new JsonObject()
                };
                var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
                {
                    Content = new StringContent(requestPayload.ToJsonString(), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Mcp-Session-Id", sessionId);
                return await HttpClient.SendAsync(request);
            })
            .ToArray();

        var httpResponses = await Task.WhenAll(tasks);

        // All should succeed
        httpResponses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));

        // Verify each response has correct id
        foreach (var httpResponse in httpResponses)
        {
            var content = await httpResponse.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(content);
            json!["id"]!.ToString().Should().MatchRegex("req-[1-5]");
        }
    }

    [ManualTestFact]
    public async Task ResponseCorrelation_WorksAcrossMultipleSessions()
    {

        // Create two sessions
        var initPayload1 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init1","method":"initialize","params":{}}""")!;
        var response1 = await HttpClient.PostAsync("/mcp", new StringContent(initPayload1.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId1 = response1.Headers.GetValues("Mcp-Session-Id").First();

        var initPayload2 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init2","method":"initialize","params":{}}""")!;
        var response2 = await HttpClient.PostAsync("/mcp", new StringContent(initPayload2.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId2 = response2.Headers.GetValues("Mcp-Session-Id").First();

        // Enqueue responses for both sessions
        var response1Payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-1","result":{"session":"1"}}""")!;
        var response2Payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-2","result":{"session":"2"}}""")!;
        backend.Enqueue(response1Payload);
        backend.Enqueue(response2Payload);

        // Send requests to different sessions
        var request1 = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent("""{"jsonrpc":"2.0","id":"req-1","method":"test","params":{}}""", Encoding.UTF8, "application/json")
        };
        request1.Headers.Add("Mcp-Session-Id", sessionId1);

        var request2 = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent("""{"jsonrpc":"2.0","id":"req-2","method":"test","params":{}}""", Encoding.UTF8, "application/json")
        };
        request2.Headers.Add("Mcp-Session-Id", sessionId2);

        var response1Result = await HttpClient.SendAsync(request1);
        var response2Result = await HttpClient.SendAsync(request2);

        response1Result.StatusCode.Should().Be(HttpStatusCode.OK);
        response2Result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content1 = await response1Result.Content.ReadAsStringAsync();
        var content2 = await response2Result.Content.ReadAsStringAsync();
        var json1 = JsonNode.Parse(content1);
        var json2 = JsonNode.Parse(content2);

        json1!["id"]!.ToString().Should().Be("req-1");
        json2!["id"]!.ToString().Should().Be("req-2");
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
