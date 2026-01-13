using Eleon.McpGateway.Host.Sse;
using FluentAssertions;

namespace Eleon.McpGateway.Host.Sse.Tests;

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
