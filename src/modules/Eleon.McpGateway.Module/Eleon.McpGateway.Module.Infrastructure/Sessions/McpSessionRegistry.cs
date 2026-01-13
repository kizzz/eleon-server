using System.Collections.Concurrent;
using Eleon.McpGateway.Module.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class McpSessionRegistry : IMcpSessionRegistry
{
    private readonly ConcurrentDictionary<string, McpSessionState> sessions = new();
    private readonly McpBackendFactoryRegistry factoryRegistry;
    private readonly ILogger<McpSessionRegistry> logger;
    private readonly McpSessionOptions options;

    public McpSessionRegistry(
        McpBackendFactoryRegistry factoryRegistry,
        IOptions<McpSessionOptions> options,
        ILogger<McpSessionRegistry> logger)
    {
        this.factoryRegistry = factoryRegistry;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<McpSessionInfo> GetOrCreateAsync(string? sessionId, string backendName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            sessionId = Guid.NewGuid().ToString("N");
            logger.LogDebug("Generated new session ID: {SessionId}", sessionId);
        }

        // Try to get existing session first
        if (sessions.TryGetValue(sessionId, out var existingState))
        {
            await existingState.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Verify backend name matches
                if (!string.Equals(existingState.BackendName, backendName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Session {sessionId} is associated with backend {existingState.BackendName}, not {backendName}.");
                }

                existingState.LastAccessedAt = DateTime.UtcNow;
                return new McpSessionInfo
                {
                    SessionId = existingState.SessionId,
                    BackendName = existingState.BackendName,
                    CreatedAt = existingState.CreatedAt,
                    LastAccessedAt = existingState.LastAccessedAt
                };
            }
            finally
            {
                existingState.Lock.Release();
            }
        }

        // Create new session
        var factory = factoryRegistry.GetFactory(backendName);
        var backend = await factory.CreateAsync(backendName, cancellationToken).ConfigureAwait(false);
        await backend.StartAsync(cancellationToken).ConfigureAwait(false);

        var newState = new McpSessionState
        {
            SessionId = sessionId,
            BackendName = backendName,
            Backend = backend,
            CreatedAt = DateTime.UtcNow,
            LastAccessedAt = DateTime.UtcNow
        };

        var addedState = sessions.GetOrAdd(sessionId, newState);
        
        // If another thread added it concurrently, dispose our backend and use the existing one
        if (addedState != newState)
        {
            logger.LogDebug("Session {SessionId} was created concurrently, disposing duplicate backend", sessionId);
            await newState.Backend.DisposeAsync().ConfigureAwait(false);
            await addedState.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                addedState.LastAccessedAt = DateTime.UtcNow;
                return new McpSessionInfo
                {
                    SessionId = addedState.SessionId,
                    BackendName = addedState.BackendName,
                    CreatedAt = addedState.CreatedAt,
                    LastAccessedAt = addedState.LastAccessedAt
                };
            }
            finally
            {
                addedState.Lock.Release();
            }
        }

        logger.LogInformation("Created new session {SessionId} for backend {BackendName}", sessionId, backendName);
        
        // Notify response correlation service about new session
        NotifySessionCreated?.Invoke(sessionId);
        
        return new McpSessionInfo
        {
            SessionId = newState.SessionId,
            BackendName = newState.BackendName,
            CreatedAt = newState.CreatedAt,
            LastAccessedAt = newState.LastAccessedAt
        };
    }

    public event Action<string>? NotifySessionCreated;

    public async Task TouchAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (!sessions.TryGetValue(sessionId, out var state))
        {
            throw new KeyNotFoundException($"Session {sessionId} not found.");
        }

        await state.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            state.LastAccessedAt = DateTime.UtcNow;
        }
        finally
        {
            state.Lock.Release();
        }
    }

    public async Task TerminateAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (!sessions.TryRemove(sessionId, out var state))
        {
            logger.LogDebug("Session {SessionId} not found for termination", sessionId);
            return;
        }

        logger.LogInformation("Terminating session {SessionId} for backend {BackendName}", sessionId, state.BackendName);

        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken = CancellationToken.None;
        }

        await state.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await state.Backend.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error disposing backend for session {SessionId}", sessionId);
        }
        finally
        {
            state.Lock.Release();
            state.Lock.Dispose();
        }
    }

    public async Task<McpSessionInfo?> TryGetAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (!sessions.TryGetValue(sessionId, out var state))
        {
            return null;
        }

        await state.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return new McpSessionInfo
            {
                SessionId = state.SessionId,
                BackendName = state.BackendName,
                CreatedAt = state.CreatedAt,
                LastAccessedAt = state.LastAccessedAt
            };
        }
        finally
        {
            state.Lock.Release();
        }
    }

    public async Task<IMcpBackend> GetBackendAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (!sessions.TryGetValue(sessionId, out var state))
        {
            throw new KeyNotFoundException($"Session {sessionId} not found.");
        }

        await state.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            state.LastAccessedAt = DateTime.UtcNow;
            return state.Backend;
        }
        finally
        {
            state.Lock.Release();
        }
    }

    internal IReadOnlyCollection<McpSessionState> GetAllSessions()
    {
        return sessions.Values.ToList();
    }

    public McpSessionState? TryGetSessionState(string sessionId)
    {
        return sessions.TryGetValue(sessionId, out var state) ? state : null;
    }
}
