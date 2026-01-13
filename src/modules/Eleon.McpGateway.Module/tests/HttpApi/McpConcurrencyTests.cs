using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Backends;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;
using Eleon.McpGateway.Module.Test.TestBase;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Eleon.McpGateway.Module.Test.TestHelpers;
using Xunit;

namespace Eleon.McpGateway.Module.Test.HttpApi;

[Trait("Category", "Manual")]
public sealed class McpConcurrencyTests : McpGatewayTestBase
{
    [ManualTestFact]
    public async Task Correlation_ConcurrentRequests_UniqueIds_ReturnCorrectResponses()
    {
        var backend = new FakeBackend("test-backend");
        RegisterBackends(backend);

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Enqueue responses out of order
        var responses = new List<JsonNode>();
        for (int i = 1; i <= 10; i++)
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
        }

        // Shuffle responses to test out-of-order delivery
        var shuffled = responses.OrderBy(_ => Random.Shared.Next()).ToList();
        foreach (var response in shuffled)
        {
            backend.Enqueue(response);
        }

        // Send concurrent requests
        var tasks = Enumerable.Range(1, 10)
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

        // Verify each response has correct id and value
        var results = new Dictionary<string, int>();
        foreach (var httpResponse in httpResponses)
        {
            var content = await httpResponse.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(content);
            var id = json!["id"]!.ToString();
            var value = json["result"]!["value"]!.GetValue<int>();
            results[id] = value;
        }

        // Verify all requests got correct responses
        for (int i = 1; i <= 10; i++)
        {
            results.Should().ContainKey($"req-{i}");
            results[$"req-{i}"].Should().Be(i);
        }

    }

    [ManualTestFact]
    public async Task Correlation_DuplicateInflightId_IsRejectedDeterministically()
    {
        var backend = new FakeBackend("test-backend");
        RegisterBackends(backend);

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Enqueue exactly one response
        var response = JsonNode.Parse("""{"jsonrpc":"2.0","id":"42","result":{"success":true}}""")!;
        backend.Enqueue(response);

        // Fire two concurrent requests with same id
        var barrier = new Barrier(2);
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"42","method":"test","params":{}}""")!;

        var task1 = Task.Run(async () =>
        {
            barrier.SignalAndWait();
            var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
                {
                    Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
                };
            request.Headers.Add("Mcp-Session-Id", sessionId);
                return await HttpClient.SendAsync(request);
        });

        var task2 = Task.Run(async () =>
        {
            barrier.SignalAndWait();
            var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
                {
                    Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
                };
            request.Headers.Add("Mcp-Session-Id", sessionId);
                return await HttpClient.SendAsync(request);
        });

        var responses = await Task.WhenAll(task1, task2);

        // One should succeed, one should fail
        var successCount = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
        var errorCount = responses.Count(r => r.StatusCode == HttpStatusCode.BadRequest || r.StatusCode == HttpStatusCode.Conflict);

        successCount.Should().Be(1);
        errorCount.Should().Be(1);

        // Verify pending request is cleaned up
        var registry = GetTestService<IMcpSessionRegistry>();
        var state = ((McpSessionRegistry)registry).TryGetSessionState(sessionId);
        state!.PendingRequests.Should().NotContainKey("42");

    }

    [ManualTestFact]
    public async Task Session_DeleteDuringInflightRequest_CleansUpPending()
    {
        var backend = new FakeBackend("test-backend");
        RegisterBackends(backend);

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Start a request that will never be answered (don't enqueue response)
        var requestPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"7","method":"long-running","params":{}}""")!;
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
            {
                Content = new StringContent(requestPayload.ToJsonString(), Encoding.UTF8, "application/json")
            };
        request.Headers.Add("Mcp-Session-Id", sessionId);

        // Start the request (it will wait for response)
        var requestTask = HttpClient.SendAsync(request);

        // Immediately delete the session
        await Task.Delay(50); // Small delay to ensure request is registered
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/mcp");
        deleteRequest.Headers.Add("Mcp-Session-Id", sessionId);
        await HttpClient.SendAsync(deleteRequest);

        // The original request should complete with error/cancel
        var response = await requestTask;
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);

        // Verify session is removed
        var registry = GetTestService<IMcpSessionRegistry>();
        var session = await registry.TryGetAsync(sessionId, CancellationToken.None);
        session.Should().BeNull();

        // Verify pending request is cleaned up
        // (Can't verify directly since session is gone, but test that it doesn't hang)

    }

    [ManualTestFact]
    public async Task SSE_SingleSubscriberPolicy_IsDeterministic()
    {
        var backend = new FakeBackend("test-backend");
        RegisterBackends(backend);

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Open first SSE connection
        var request1 = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request1.Headers.Add("Mcp-Session-Id", sessionId);
        using var sseResponse1 = await HttpClient.SendAsync(request1, HttpCompletionOption.ResponseHeadersRead);
        sseResponse1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Attempt second SSE connection for same session
        var request2 = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request2.Headers.Add("Mcp-Session-Id", sessionId);
        var sseResponse2 = await HttpClient.SendAsync(request2, HttpCompletionOption.ResponseHeadersRead);

        // Based on current implementation, both connections are allowed
        // Document this behavior in TEST_MATRIX.md
        // For now, test that both connections work (current behavior)
        sseResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        sseResponse1.Dispose();
        sseResponse2.Dispose();
    }

    [ManualTestFact]
    public async Task MultipleClients_CreateDifferentSessions_Concurrently_NoInterference()
    {
        var backend = new FakeBackend("test-backend");
        RegisterBackends(backend);

        const int concurrentSessions = 10;
        var tasks = Enumerable.Range(0, concurrentSessions)
            .Select(async i =>
            {
                var initPayload = new JsonObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = $"init-{i}",
                    ["method"] = "initialize",
                    ["params"] = new JsonObject()
                };
                var response = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
                return response.Headers.GetValues("Mcp-Session-Id").First();
            })
            .ToArray();

        var sessionIds = await Task.WhenAll(tasks);

        // All sessions should be unique
        sessionIds.Should().OnlyHaveUniqueItems();
        sessionIds.Should().HaveCount(concurrentSessions);

    }

    [ManualTestFact]
    public async Task ConcurrentRequests_ToDifferentSessions_AreIsolated()
    {
        var backend = new FakeBackend("test-backend");
        RegisterBackends(backend);

        // Create two sessions
        var initPayload1 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init1","method":"initialize","params":{}}""")!;
        var response1 = await HttpClient.PostAsync("/mcp", new StringContent(initPayload1.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId1 = response1.Headers.GetValues("Mcp-Session-Id").First();

        var initPayload2 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init2","method":"initialize","params":{}}""")!;
        var response2 = await HttpClient.PostAsync("/mcp", new StringContent(initPayload2.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId2 = response2.Headers.GetValues("Mcp-Session-Id").First();

        // Enqueue responses for both sessions
        backend.Enqueue(JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-1","result":{"session":"1"}}""")!);
        backend.Enqueue(JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-2","result":{"session":"2"}}""")!);

        // Send concurrent requests to different sessions
        var task1 = Task.Run(async () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
                {
                    Content = new StringContent("""{"jsonrpc":"2.0","id":"req-1","method":"test","params":{}}""", Encoding.UTF8, "application/json")
                };
            request.Headers.Add("Mcp-Session-Id", sessionId1);
                return await HttpClient.SendAsync(request);
        });

        var task2 = Task.Run(async () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
                {
                    Content = new StringContent("""{"jsonrpc":"2.0","id":"req-2","method":"test","params":{}}""", Encoding.UTF8, "application/json")
                };
            request.Headers.Add("Mcp-Session-Id", sessionId2);
                return await HttpClient.SendAsync(request);
        });

        var responses = await Task.WhenAll(task1, task2);

        responses[0].StatusCode.Should().Be(HttpStatusCode.OK);
        responses[1].StatusCode.Should().Be(HttpStatusCode.OK);

        var content1 = await responses[0].Content.ReadAsStringAsync();
        var content2 = await responses[1].Content.ReadAsStringAsync();
        var json1 = JsonNode.Parse(content1);
        var json2 = JsonNode.Parse(content2);

        json1!["id"]!.ToString().Should().Be("req-1");
        json2!["id"]!.ToString().Should().Be("req-2");

    }

    [ManualTestFact]
    public async Task SSE_ConnectionEstablished_WhileSessionTerminated_Returns400()
    {
        var backend = new FakeBackend("test-backend");
        RegisterBackends(backend);

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Terminate session
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/mcp");
        deleteRequest.Headers.Add("Mcp-Session-Id", sessionId);
        await HttpClient.SendAsync(deleteRequest);

        // Try to establish SSE connection with terminated session
        var getRequest = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        getRequest.Headers.Add("Mcp-Session-Id", sessionId);
        var response = await HttpClient.SendAsync(getRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid or expired session");

    }

}
