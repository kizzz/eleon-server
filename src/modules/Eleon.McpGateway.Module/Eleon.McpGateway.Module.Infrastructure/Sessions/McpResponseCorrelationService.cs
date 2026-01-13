using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using Eleon.Logging.Lib.VportalLogging;
using Eleon.McpGateway.Module.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class McpResponseCorrelationService : IHostedService
{
    private readonly IMcpSessionRegistry sessionRegistry;
    private readonly McpRequestCorrelationService correlationService;
    private readonly ILogger<McpResponseCorrelationService> logger;
    private readonly IBoundaryLogger boundaryLogger;
    private IBoundaryScope? boundaryScope;
    private readonly ConcurrentDictionary<string, Task> activeSubscriptions = new();
    private CancellationTokenSource? cancellationTokenSource;
    private McpSessionRegistry? concreteRegistry;

    public McpResponseCorrelationService(
        IMcpSessionRegistry sessionRegistry,
        McpRequestCorrelationService correlationService,
        ILogger<McpResponseCorrelationService> logger,
        IBoundaryLogger boundaryLogger)
    {
        this.sessionRegistry = sessionRegistry;
        this.correlationService = correlationService;
        this.logger = logger;
        this.boundaryLogger = boundaryLogger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        boundaryScope = boundaryLogger.Begin("HostedService McpResponseCorrelationService");
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        if (sessionRegistry is not McpSessionRegistry concreteRegistry)
        {
            logger.LogWarning("Session registry is not McpSessionRegistry, response correlation will not work");
            return Task.CompletedTask;
        }

        this.concreteRegistry = concreteRegistry;
        concreteRegistry.NotifySessionCreated += SubscribeToSession;

        // Subscribe to existing sessions
        foreach (var state in concreteRegistry.GetAllSessions())
        {
            _ = SubscribeToSessionAsync(state.SessionId, cancellationTokenSource.Token);
        }

        logger.LogInformation("MCP response correlation service started");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping MCP response correlation service");
        cancellationTokenSource?.Cancel();
        if (concreteRegistry is not null)
        {
            concreteRegistry.NotifySessionCreated -= SubscribeToSession;
            concreteRegistry = null;
        }

        var tasks = activeSubscriptions.Values.ToArray();
        if (tasks.Length > 0)
        {
            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        activeSubscriptions.Clear();
        cancellationTokenSource?.Dispose();
        boundaryScope?.Dispose();
        boundaryScope = null;
    }

    public void SubscribeToSession(string sessionId)
    {
        if (cancellationTokenSource is null || cancellationTokenSource.IsCancellationRequested)
        {
            logger.LogDebug("Cannot subscribe to session {SessionId} - service not started", sessionId);
            return;
        }

        if (activeSubscriptions.ContainsKey(sessionId))
        {
            logger.LogDebug("Session {SessionId} is already being monitored", sessionId);
            return;
        }

        logger.LogDebug("Subscribing to session {SessionId} for response correlation", sessionId);
        _ = SubscribeToSessionAsync(sessionId, cancellationTokenSource.Token);
    }

    private async Task SubscribeToSessionAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (sessionRegistry is not McpSessionRegistry concreteRegistry)
        {
            return;
        }

        var state = concreteRegistry.TryGetSessionState(sessionId);
        if (state is null)
        {
            logger.LogWarning("Session {SessionId} not found for subscription", sessionId);
            return;
        }

        var subscriptionTask = Task.Run(async () =>
        {
            try
            {
                logger.LogDebug("Starting response correlation for session {SessionId}", sessionId);
                await foreach (var message in state.Backend.ReceiveAsync(cancellationToken).WithCancellation(cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    await ProcessMessageAsync(sessionId, message, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in response correlation for session {SessionId}", sessionId);
            }
            finally
            {
                activeSubscriptions.TryRemove(sessionId, out _);
                logger.LogDebug("Stopped response correlation for session {SessionId}", sessionId);
            }
        }, cancellationToken);

        activeSubscriptions.TryAdd(sessionId, subscriptionTask);
    }

    private async Task ProcessMessageAsync(string sessionId, JsonNode message, CancellationToken cancellationToken)
    {
        try
        {
            // Extract id from JSON-RPC message
            var idNode = message["id"];
            if (idNode is null)
            {
                // This is a notification, not a response - ignore for correlation
                return;
            }

            var requestId = idNode.ToString();
            if (string.IsNullOrWhiteSpace(requestId))
            {
                return;
            }

            // Check if this is a response (has result or error) vs a request
            var hasResult = message["result"] is not null;
            var hasError = message["error"] is not null;

            if (!hasResult && !hasError)
            {
                // This is a request, not a response - ignore
                return;
            }

            logger.LogDebug("Received response for request {RequestId} in session {SessionId}", requestId, sessionId);
            correlationService.CompleteRequest(sessionId, requestId, message);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error processing message for session {SessionId}", sessionId);
        }
    }
}
