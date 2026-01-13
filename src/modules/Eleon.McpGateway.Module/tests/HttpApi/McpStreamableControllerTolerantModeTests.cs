using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Test.TestBase;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Eleon.McpGateway.Module.Test.TestHelpers;
using Xunit;

namespace Eleon.McpGateway.Module.Test.HttpApi;

[Trait("Category", "Manual")]
public sealed class McpStreamableControllerTolerantModeOffTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new("test-backend");

    public McpStreamableControllerTolerantModeOffTests()
    {
        RegisterBackends(backend);
    }

    [ManualTestFact]
    public async Task TolerantMode_Off_RequiresSessionHeader()
    {
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Mcp-Session-Id header required");

    }
}

[Trait("Category", "Manual")]
public sealed class McpStreamableControllerTolerantModeOnTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new("test-backend");

    public McpStreamableControllerTolerantModeOnTests()
    {
        RegisterBackends(backend);
    }

    protected override IReadOnlyDictionary<string, string?> GetConfigurationOverrides()
    {
        return new Dictionary<string, string?>
        {
            ["McpStreamable:TolerantMode"] = "true"
        };
    }

    [ManualTestFact]
    public async Task TolerantMode_On_AutoCreatesSession_ForNonInitializeRequests()
    {
        // Note: TolerantMode tests need custom configuration - using default setup for now
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
        response.Headers.Should().ContainKey("Mcp-Session-Id");
        var sessionId = response.Headers.GetValues("Mcp-Session-Id").First();
        sessionId.Should().NotBeNullOrWhiteSpace();

    }

    [ManualTestFact]
    public async Task TolerantMode_On_ReturnsSessionId_InResponseHeader()
    {
        // Note: TolerantMode tests need custom configuration - using default setup for now
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;

        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        response.Headers.Should().ContainKey("Mcp-Session-Id");
        var sessionId = response.Headers.GetValues("Mcp-Session-Id").First();
        sessionId.Should().NotBeNullOrWhiteSpace();

    }

    [ManualTestFact]
    public async Task TolerantMode_On_SessionCanBeReused()
    {
        // Note: TolerantMode tests need custom configuration - using default setup for now

        // First request without session header (auto-creates)
        var payload1 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test1","params":{}}""")!;
        var response1 = await HttpClient.PostAsync("/mcp", new StringContent(payload1.ToJsonString(), Encoding.UTF8, "application/json"));
        var sessionId = response1.Headers.GetValues("Mcp-Session-Id").First();

        // Second request with session header (reuses)
        var payload2 = JsonNode.Parse("""{"jsonrpc":"2.0","id":"2","method":"test2","params":{}}""")!;
        var request2 = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(payload2.ToJsonString(), Encoding.UTF8, "application/json")
        };
        request2.Headers.Add("Mcp-Session-Id", sessionId);

        var response2 = await HttpClient.SendAsync(request2);
        response2.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
        response2.Headers.GetValues("Mcp-Session-Id").First().Should().Be(sessionId);

    }

    [ManualTestFact]
    public async Task TolerantMode_Configuration_IsReadCorrectly()
    {
        // Note: TolerantMode tests need custom configuration - using default setup for now

        // Verify tolerant mode is enabled by making a request without session header
        var payload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"test","params":{}}""")!;
        var response = await HttpClient.PostAsync("/mcp", new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"));

        // Should succeed (not return 400)
        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Mcp-Session-Id");

    }

}

// TolerantModeGatewayWebApplicationFactory removed - use McpGatewayTestBase with custom configuration if needed
