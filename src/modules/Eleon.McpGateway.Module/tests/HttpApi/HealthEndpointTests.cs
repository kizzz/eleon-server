using System.Net;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Test.TestBase;
using Eleon.McpGateway.Module.Test.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Eleon.McpGateway.Module.Test.HttpApi;

[Trait("Category", "Manual")]
public sealed class HealthEndpointTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new();

    public HealthEndpointTests()
    {
        RegisterBackends(backend);
    }

    [ManualTestFact]
    public async Task GetHealth_ReturnsOkPayload()
    {
        var response = await HttpClient.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        json?["status"]?.GetValue<string>().Should().Be("ok");
    }
}
