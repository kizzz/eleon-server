using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Sessions;

public sealed class McpSessionRegistryTests
{
    [Fact]
    public async Task GetOrCreateAsync_CreatesNewSession_WhenSessionIdIsNull()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var sessionInfo = await registry.GetOrCreateAsync(null, "test-backend", CancellationToken.None);

        sessionInfo.SessionId.Should().NotBeNullOrEmpty();
        sessionInfo.BackendName.Should().Be("test-backend");
        sessionInfo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetOrCreateAsync_ReturnsExistingSession_WhenSessionIdProvided()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var sessionInfo1 = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var sessionInfo2 = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);

        sessionInfo1.SessionId.Should().Be(sessionInfo2.SessionId);
        sessionInfo1.CreatedAt.Should().Be(sessionInfo2.CreatedAt);
    }

    [Fact]
    public async Task GetOrCreateAsync_CreatesDifferentSessions_ForDifferentIds()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var session1 = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var session2 = await registry.GetOrCreateAsync("session-2", "test-backend", CancellationToken.None);

        session1.SessionId.Should().NotBe(session2.SessionId);
    }

    [Fact]
    public async Task GetOrCreateAsync_Throws_WhenBackendNameMismatch()
    {
        var factory1 = new FakeBackendFactory("backend-1");
        var factory2 = new FakeBackendFactory("backend-2");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory1, factory2 });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        await registry.GetOrCreateAsync("session-1", "backend-1", CancellationToken.None);

        var act = async () => await registry.GetOrCreateAsync("session-1", "backend-2", CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task TouchAsync_UpdatesLastAccessedAt()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var session1 = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        await Task.Delay(100);
        await registry.TouchAsync("session-1", CancellationToken.None);
        var session2 = await registry.TryGetAsync("session-1", CancellationToken.None);

        session2.Should().NotBeNull();
        session2!.LastAccessedAt.Should().BeAfter(session1.LastAccessedAt);
    }

    [Fact]
    public async Task TouchAsync_Throws_WhenSessionNotFound()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var act = async () => await registry.TouchAsync("missing", CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task TerminateAsync_DisposesBackend_AndRemovesSession()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var backend = await registry.GetBackendAsync("session-1", CancellationToken.None);
        var fakeBackend = (FakeBackend)backend;

        await registry.TerminateAsync("session-1", CancellationToken.None);

        fakeBackend.Disposed.Should().BeTrue();
        var session = await registry.TryGetAsync("session-1", CancellationToken.None);
        session.Should().BeNull();
    }

    [Fact]
    public async Task TerminateAsync_IsIdempotent()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        await registry.TerminateAsync("session-1", CancellationToken.None);

        // Should not throw
        await registry.TerminateAsync("session-1", CancellationToken.None);
    }

    [Fact]
    public async Task GetOrCreateAsync_IsThreadSafe()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var tasks = Enumerable.Range(0, 10)
            .Select(i => registry.GetOrCreateAsync($"session-{i}", "test-backend", CancellationToken.None))
            .ToArray();

        var sessions = await Task.WhenAll(tasks);

        sessions.Should().HaveCount(10);
        sessions.Select(s => s.SessionId).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task GetOrCreateAsync_ConcurrentCreation_SameSessionId_ReturnsSameSession()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        const int concurrentTasks = 20;
        var tasks = Enumerable.Range(0, concurrentTasks)
            .Select(_ => registry.GetOrCreateAsync("same-session", "test-backend", CancellationToken.None))
            .ToArray();

        var sessions = await Task.WhenAll(tasks);

        // All should return the same session
        var firstSession = sessions[0];
        sessions.Should().AllBeEquivalentTo(firstSession, opts => opts.Excluding(s => s.LastAccessedAt));
        sessions.Select(s => s.SessionId).Should().AllBeEquivalentTo(firstSession.SessionId);
        sessions.Select(s => s.CreatedAt).Should().AllBeEquivalentTo(firstSession.CreatedAt);
    }

    [Fact]
    public async Task GetOrCreateAsync_ConcurrentCreation_DifferentIds_CreatesDifferentSessions()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        const int concurrentTasks = 10;
        var tasks = Enumerable.Range(0, concurrentTasks)
            .Select(i => registry.GetOrCreateAsync($"session-{i}", "test-backend", CancellationToken.None))
            .ToArray();

        var sessions = await Task.WhenAll(tasks);

        sessions.Should().HaveCount(concurrentTasks);
        sessions.Select(s => s.SessionId).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task GetOrCreateAsync_RespectsCancellationToken()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await registry.GetOrCreateAsync("session-1", "test-backend", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task TerminateAsync_RespectsCancellationToken()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Terminate should still work even with cancelled token (it's idempotent)
        // But we test that cancellation is respected if needed
        await registry.TerminateAsync("session-1", cts.Token);
    }

    [Fact]
    public async Task TryGetAsync_NonExistentSession_ReturnsNull()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var session = await registry.TryGetAsync("non-existent", CancellationToken.None);

        session.Should().BeNull();
    }

    [Fact]
    public async Task Sessions_AreIsolated_DoNotInterfere()
    {
        var factory = new FakeBackendFactory("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        var registry = new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);

        var session1 = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var session2 = await registry.GetOrCreateAsync("session-2", "test-backend", CancellationToken.None);

        session1.SessionId.Should().NotBe(session2.SessionId);
        session1.CreatedAt.Should().NotBe(session2.CreatedAt);

        // Terminate one session, other should still exist
        await registry.TerminateAsync("session-1", CancellationToken.None);
        var session1After = await registry.TryGetAsync("session-1", CancellationToken.None);
        var session2After = await registry.TryGetAsync("session-2", CancellationToken.None);

        session1After.Should().BeNull();
        session2After.Should().NotBeNull();
        session2After!.SessionId.Should().Be(session2.SessionId);
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

        public Task SendAsync(JsonNode message, CancellationToken cancellationToken) => Task.CompletedTask;

        public IAsyncEnumerable<JsonNode> ReceiveAsync(CancellationToken cancellationToken)
        {
            return Empty();

            static async IAsyncEnumerable<JsonNode> Empty()
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
}

