using Eleon.McpGateway.Host.Sse;
using FluentAssertions;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class SseConnectionCoordinatorTests
{
    [Fact]
    public void TryAcquire_AllowsMultipleConnections()
    {
        var coordinator = new SseConnectionCoordinator();

        coordinator.TryAcquire("a").Should().BeTrue();
        coordinator.TryAcquire("a").Should().BeTrue();
        coordinator.TryAcquire("b").Should().BeTrue();
    }
}
