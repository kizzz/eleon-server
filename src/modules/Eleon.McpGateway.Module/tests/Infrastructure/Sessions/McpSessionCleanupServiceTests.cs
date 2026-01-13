using Eleon.Logging.Lib.VportalLogging;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Sessions;

public sealed class McpSessionCleanupServiceTests
{
    [Fact]
    public async Task CleanupService_TerminatesExpiredSessions()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions
        {
            SessionTtl = TimeSpan.FromMilliseconds(100),
            CleanupInterval = TimeSpan.FromMilliseconds(50)
        });
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);
        var boundaryLogger = CreateBoundaryLogger();
        var cleanupService = new McpSessionCleanupService(registry, options, NullLogger<McpSessionCleanupService>.Instance, boundaryLogger);

        var session1 = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var backend1 = await registry.GetBackendAsync("session-1", CancellationToken.None);
        var fakeBackend1 = (FakeBackend)backend1;

        // Wait for session to expire
        await Task.Delay(150);

        // Start cleanup service
        var cts = new CancellationTokenSource();
        await cleanupService.StartAsync(cts.Token);

        // Wait for cleanup to run
        await Task.Delay(100);

        // Verify session was terminated
        await Task.Delay(100); // Give cleanup time to complete
        fakeBackend1.Disposed.Should().BeTrue();
        var session = await registry.TryGetAsync("session-1", CancellationToken.None);
        session.Should().BeNull();

        await cleanupService.StopAsync(cts.Token);
        cts.Dispose();
    }

    [Fact]
    public async Task CleanupService_PreservesActiveSessions()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions
        {
            SessionTtl = TimeSpan.FromMilliseconds(500),
            CleanupInterval = TimeSpan.FromMilliseconds(50)
        });
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);
        var boundaryLogger = CreateBoundaryLogger();
        var cleanupService = new McpSessionCleanupService(registry, options, NullLogger<McpSessionCleanupService>.Instance, boundaryLogger);

        var session1 = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var backend1 = await registry.GetBackendAsync("session-1", CancellationToken.None);
        var fakeBackend1 = (FakeBackend)backend1;

        // Start cleanup service
        var cts = new CancellationTokenSource();
        await cleanupService.StartAsync(cts.Token);

        // Keep session active by touching it
        await Task.Delay(100);
        await registry.TouchAsync("session-1", CancellationToken.None);
        await Task.Delay(100);
        await registry.TouchAsync("session-1", CancellationToken.None);

        // Wait for cleanup cycles
        await Task.Delay(150);

        // Verify session is still alive
        fakeBackend1.Disposed.Should().BeFalse();
        var session = await registry.TryGetAsync("session-1", CancellationToken.None);
        session.Should().NotBeNull();

        await cleanupService.StopAsync(cts.Token);
        cts.Dispose();
    }

    [Fact]
    public async Task CleanupService_RespectsCancellationToken()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions
        {
            SessionTtl = TimeSpan.FromMinutes(30),
            CleanupInterval = TimeSpan.FromMilliseconds(50)
        });
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);
        var boundaryLogger = CreateBoundaryLogger();
        var cleanupService = new McpSessionCleanupService(registry, options, NullLogger<McpSessionCleanupService>.Instance, boundaryLogger);

        var cts = new CancellationTokenSource();
        await cleanupService.StartAsync(cts.Token);

        // Cancel after a short delay
        await Task.Delay(100);
        cts.Cancel();

        // Stop should complete without hanging
        await cleanupService.StopAsync(CancellationToken.None);
        cts.Dispose();
    }

    private sealed class FakeBackendFactory : IMcpBackendFactory
    {
        public FakeBackendFactory(string backendName)
        {
            BackendName = backendName;
        }

        public string BackendName { get; }

        public Task<IMcpBackend> CreateAsync(string backendName, CancellationToken cancellationToken)
        {
            return Task.FromResult<IMcpBackend>(new FakeBackend(BackendName));
        }
    }

    private sealed class FakeBackend : IMcpBackend
    {
        public string Name { get; }
        public bool Disposed { get; private set; }

        public FakeBackend(string name)
        {
            Name = name;
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SendAsync(System.Text.Json.Nodes.JsonNode message, CancellationToken cancellationToken) => Task.CompletedTask;

        public IAsyncEnumerable<System.Text.Json.Nodes.JsonNode> ReceiveAsync(CancellationToken cancellationToken)
        {
            return Empty();

            static async IAsyncEnumerable<System.Text.Json.Nodes.JsonNode> Empty()
            {
                await Task.CompletedTask;
                yield break;
            }
        }

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return ValueTask.CompletedTask;
        }
    }

    private static IBoundaryLogger CreateBoundaryLogger()
    {
        var scopeFactory = new VportalOperationScopeFactory(NullLogger<VportalOperationScopeFactory>.Instance);
        return new BoundaryLogger(scopeFactory, NullLogger<BoundaryLogger>.Instance);
    }
}
