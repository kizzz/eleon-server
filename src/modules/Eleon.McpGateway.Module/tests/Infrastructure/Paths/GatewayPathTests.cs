using Eleon.Mcp.Infrastructure.Paths;
using FluentAssertions;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Paths;

public sealed class GatewayPathTests
{
    [Theory]
    [InlineData(null, "/sse")]
    [InlineData("", "/sse")]
    [InlineData("stream", "/stream")]
    [InlineData("/stream/", "/stream")]
    public void Normalize_ProducesLeadingSlashAndTrimsTrailing(string? input, string expected)
    {
        GatewayPath.Normalize(input).Should().Be(expected);
    }
}

