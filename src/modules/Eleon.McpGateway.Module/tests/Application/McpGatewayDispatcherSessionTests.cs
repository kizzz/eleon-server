using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Application.Services;
using Eleon.McpGateway.Module.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.McpGateway.Module.Test.Application;

public sealed class McpGatewayDispatcherSessionTests
{
    [Fact]
    public async Task ForwardAsync_UsesSessionBackend_WhenSessionIdProvided()
    {
        var singletonBackend = new RecordingBackend("singleton");
        var registry = new SingleBackendRegistry(singletonBackend);
        var sessionRegistry = new FakeSessionRegistry();
        var dispatcher = new McpGatewayDispatcherAppService(registry, sessionRegistry, NullLogger<McpGatewayDispatcherAppService>.Instance);

        var payload = JsonNode.Parse("""{"id":1,"method":"test"}""")!;
        await dispatcher.ForwardAsync(payload, "test-backend", "session-1", CancellationToken.None);

        sessionRegistry.LastUsedSessionId.Should().Be("session-1");
        sessionRegistry.LastUsedBackendName.Should().Be("test-backend");
        singletonBackend.Sent.Should().BeEmpty(); // Should not use singleton
    }

    [Fact]
    public async Task ForwardAsync_FallsBackToSingleton_WhenSessionIdNull()
    {
        var singletonBackend = new RecordingBackend("singleton");
        var registry = new SingleBackendRegistry(singletonBackend);
        var sessionRegistry = new FakeSessionRegistry();
        var dispatcher = new McpGatewayDispatcherAppService(registry, sessionRegistry, NullLogger<McpGatewayDispatcherAppService>.Instance);

        var payload = JsonNode.Parse("""{"id":1,"method":"test"}""")!;
        await dispatcher.ForwardAsync(payload, "singleton", null, CancellationToken.None);

        singletonBackend.Sent.Should().ContainSingle();
        sessionRegistry.LastUsedSessionId.Should().BeNull();
    }

    [Fact]
    public async Task GetOutboundStream_UsesSessionBackend_WhenSessionIdProvided()
    {
        var singletonBackend = new RecordingBackend("singleton");
        var registry = new SingleBackendRegistry(singletonBackend);
        var sessionRegistry = new FakeSessionRegistry();
        var dispatcher = new McpGatewayDispatcherAppService(registry, sessionRegistry, NullLogger<McpGatewayDispatcherAppService>.Instance);

        var stream = dispatcher.GetOutboundStream("test-backend", "session-1", CancellationToken.None);
        var count = 0;
        await foreach (var _ in stream)
        {
            count++;
        }

        sessionRegistry.LastUsedSessionId.Should().Be("session-1");
        sessionRegistry.LastUsedBackendName.Should().Be("test-backend");
    }

    [Fact]
    public async Task ForwardAsync_CallsTouchAsync_OnSessionAccess()
    {
        var singletonBackend = new RecordingBackend("singleton");
        var registry = new SingleBackendRegistry(singletonBackend);
        var sessionRegistry = new FakeSessionRegistry();
        var dispatcher = new McpGatewayDispatcherAppService(registry, sessionRegistry, NullLogger<McpGatewayDispatcherAppService>.Instance);

        var payload = JsonNode.Parse("""{"id":1,"method":"test"}""")!;
        await dispatcher.ForwardAsync(payload, "test-backend", "session-1", CancellationToken.None);

        sessionRegistry.TouchCallCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task MultipleSessions_CanCoexist()
    {
        var singletonBackend = new RecordingBackend("singleton");
        var registry = new SingleBackendRegistry(singletonBackend);
        var sessionRegistry = new FakeSessionRegistry();
        var dispatcher = new McpGatewayDispatcherAppService(registry, sessionRegistry, NullLogger<McpGatewayDispatcherAppService>.Instance);

        var payload1 = JsonNode.Parse("""{"id":1,"method":"test1"}""")!;
        var payload2 = JsonNode.Parse("""{"id":2,"method":"test2"}""")!;

        await dispatcher.ForwardAsync(payload1, "test-backend", "session-1", CancellationToken.None);
        await dispatcher.ForwardAsync(payload2, "test-backend", "session-2", CancellationToken.None);

        sessionRegistry.CreatedSessions.Should().Contain("session-1", "session-2");
    }

    private sealed class SingleBackendRegistry : IMcpBackendRegistry
    {
        private readonly IMcpBackend backend;
        public SingleBackendRegistry(IMcpBackend backend) => this.backend = backend;
        public IMcpBackend GetBackend(string name)
        {
            if (!string.Equals(name, backend.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new KeyNotFoundException(name);
            }

            return backend;
        }
        public IMcpBackend GetDefaultBackend() => backend;
        public IReadOnlyCollection<IMcpBackend> GetAll() => new[] { backend };
    }

    private sealed class RecordingBackend : IMcpBackend
    {
        public string Name { get; }
        public List<JsonNode> Sent { get; } = new();

        public RecordingBackend(string name)
        {
            Name = name;
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SendAsync(JsonNode message, CancellationToken cancellationToken)
        {
            Sent.Add(message.DeepClone() ?? message);
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<JsonNode> ReceiveAsync(CancellationToken cancellationToken)
        {
            return Empty();

            static async IAsyncEnumerable<JsonNode> Empty()
            {
                await Task.CompletedTask;
                yield break;
            }
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class FakeSessionRegistry : IMcpSessionRegistry
    {
        public string? LastUsedSessionId { get; private set; }
        public string? LastUsedBackendName { get; private set; }
        public int TouchCallCount { get; private set; }
        public HashSet<string> CreatedSessions { get; } = new();

        private readonly Dictionary<string, IMcpBackend> backends = new();

        public async Task<McpSessionInfo> GetOrCreateAsync(string? sessionId, string backendName, CancellationToken cancellationToken)
        {
            LastUsedSessionId = sessionId;
            LastUsedBackendName = backendName;

            if (sessionId is not null)
            {
                CreatedSessions.Add(sessionId);
            }

            if (sessionId is not null && !backends.ContainsKey(sessionId))
            {
                backends[sessionId] = new FakeSessionBackend(backendName);
            }

            var actualSessionId = sessionId ?? Guid.NewGuid().ToString("N");
            return new McpSessionInfo
            {
                SessionId = actualSessionId,
                BackendName = backendName,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow
            };
        }

        public Task TouchAsync(string sessionId, CancellationToken cancellationToken)
        {
            TouchCallCount++;
            return Task.CompletedTask;
        }

        public Task TerminateAsync(string sessionId, CancellationToken cancellationToken)
        {
            backends.Remove(sessionId);
            return Task.CompletedTask;
        }

        public Task<McpSessionInfo?> TryGetAsync(string sessionId, CancellationToken cancellationToken)
        {
            if (backends.ContainsKey(sessionId))
            {
                return Task.FromResult<McpSessionInfo?>(new McpSessionInfo
                {
                    SessionId = sessionId,
                    BackendName = "test-backend",
                    CreatedAt = DateTime.UtcNow,
                    LastAccessedAt = DateTime.UtcNow
                });
            }

            return Task.FromResult<McpSessionInfo?>(null);
        }

        public Task<IMcpBackend> GetBackendAsync(string sessionId, CancellationToken cancellationToken)
        {
            if (backends.TryGetValue(sessionId, out var backend))
            {
                return Task.FromResult(backend);
            }

            throw new KeyNotFoundException($"Session {sessionId} not found");
        }
    }

    private sealed class FakeSessionBackend : IMcpBackend
    {
        public string Name { get; }

        public FakeSessionBackend(string name)
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

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

