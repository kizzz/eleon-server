using System.Threading;
using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;

/// <summary>
/// Fake backend factory that tracks CreatedCount (number of backend instances created).
/// Creates new FakeBackendForConcurrency instances (session-scoped).
/// </summary>
internal class FakeBackendFactoryForConcurrency : IMcpBackendFactory
{
    protected int createdCount = 0;

    public FakeBackendFactoryForConcurrency(string backendName = "test")
    {
        BackendName = backendName;
    }

    public string BackendName { get; }

    public int CreatedCount => Volatile.Read(ref createdCount);

    public virtual Task<IMcpBackend> CreateAsync(string backendName, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref createdCount);
        return Task.FromResult<IMcpBackend>(new FakeBackendForConcurrency(BackendName));
    }
}
