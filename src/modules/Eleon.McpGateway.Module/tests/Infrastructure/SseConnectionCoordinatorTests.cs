using Eleon.McpGateway.Module.Infrastructure;
using FluentAssertions;

namespace Eleon.McpGateway.Module.Test.Infrastructure;

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

