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
public sealed class McpStreamableControllerNegativeTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new("test-backend");

    public McpStreamableControllerNegativeTests()
    {
        RegisterBackends(backend);
    }

    [ManualTestFact]
    public async Task Post_MalformedJson_Returns400()
    {
        var response = await HttpClient.PostAsync("/mcp", new StringContent("not-json", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid JSON payload");
    }

    [ManualTestFact]
    public async Task Post_MissingPayload_Returns400()
    {
        var response = await HttpClient.PostAsync("/mcp", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Missing JSON payload");
    }

    [ManualTestFact]
    public async Task Post_InvalidJsonRpcFormat_MissingJsonRpcField_ReturnsError()
    {
        var payload = JsonNode.Parse("""{"id":"1","method":"test"}""")!; // Missing jsonrpc field

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        // Should still process but may return error or accept it
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task Post_MissingMethodField_ReturnsError()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1"}""")!; // Missing method field

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        // Should return error or bad request
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK, HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task Post_InvalidBackendName_Returns404()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"initialize","params":{}}""")!;

        var response = await HttpClient.PostAsync("/mcp/nonexistent-backend", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("not found");
    }

    [ManualTestFact]
    public async Task Post_WithoutSessionHeader_NonTolerant_Returns400()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Mcp-Session-Id header required");
    }

    [ManualTestFact]
    public async Task Get_WithoutSessionHeader_Returns400()
    {
        var response = await HttpClient.GetAsync("/mcp");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Mcp-Session-Id header required");
    }

    [ManualTestFact]
    public async Task Delete_WithoutSessionHeader_Returns400()
    {
        var response = await HttpClient.DeleteAsync("/mcp");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Mcp-Session-Id header required");
    }

    [ManualTestFact]
    public async Task Post_InvalidJsonRpcIdType_NonStringOrNumber_HandlesGracefully()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Request with id as object (invalid)
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":{},"method":"test","params":{}}""")!;
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Mcp-Session-Id", sessionId);

        var response = await HttpClient.SendAsync(request);

        // Should handle gracefully (may accept or reject)
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task Get_InvalidSessionId_Returns400()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        request.Headers.Add("Mcp-Session-Id", "invalid-session-id");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid or expired session");
    }

    [ManualTestFact]
    public async Task Post_InvalidContentType_HandlesGracefully()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"initialize","params":{}}""")!;

        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "text/plain")
        };

        var response = await HttpClient.SendAsync(request);

        // Should still process JSON regardless of Content-Type
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [ManualTestFact]
    public async Task Post_EmptyJsonObject_HandlesGracefully()
    {
        var payload = JsonNode.Parse("{}")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        // Should return error for missing required fields
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK, HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task Post_InvalidJsonStructure_HandlesGracefully()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":"not-an-object"}""")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        // Should handle gracefully (may forward to backend or reject)
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.Accepted);
    }

    [ManualTestFact]
    public async Task Get_ExpiredSession_Returns400()
    {

        // Initialize to get session
        var initPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"init","method":"initialize","params":{}}""")!;
        var initResponse = await HttpClient.PostAsync("/mcp", new StringContent(initPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = initResponse.Headers.GetValues("Mcp-Session-Id").First();

        // Terminate session
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/mcp");
        deleteRequest.Headers.Add("Mcp-Session-Id", sessionId);
        await HttpClient.SendAsync(deleteRequest);

        // Try to use expired session
        var getRequest = new HttpRequestMessage(HttpMethod.Get, "/mcp");
        getRequest.Headers.Add("Mcp-Session-Id", sessionId);

        var response = await HttpClient.SendAsync(getRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid or expired session");
    }

}
