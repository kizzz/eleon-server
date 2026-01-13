using System.Net;
using System.Text.Json.Nodes;
using FluentAssertions;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class HealthEndpointTests : IAsyncDisposable
{
    private readonly FakeBackend backend = new();
    private readonly GatewayWebApplicationFactory factory;

    public HealthEndpointTests()
    {
        factory = new GatewayWebApplicationFactory(backend);
    }

    [Fact]
    public async Task GetHealth_ReturnsOkPayload()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        json?["status"]?.GetValue<string>().Should().Be("ok");
    }

    public async ValueTask DisposeAsync()
    {
        await factory.DisposeAsync();
        await backend.DisposeAsync();
    }
}
