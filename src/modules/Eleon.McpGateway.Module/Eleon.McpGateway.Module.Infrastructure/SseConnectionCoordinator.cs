namespace Eleon.McpGateway.Module.Infrastructure;

public sealed class SseConnectionCoordinator
{
    public bool TryAcquire(string backend) => true;

    public void Release(string backend)
    {
        // no-op: multi-connection allowed
    }
}

