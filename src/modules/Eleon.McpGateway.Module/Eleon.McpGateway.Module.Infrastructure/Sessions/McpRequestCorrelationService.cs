using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class McpRequestCorrelationService
{
    private readonly IMcpSessionRegistry sessionRegistry;
    private readonly McpStreamableOptions options;
    private readonly ILogger<McpRequestCorrelationService> logger;

    public McpRequestCorrelationService(
        IMcpSessionRegistry sessionRegistry,
        IOptions<McpStreamableOptions> options,
        ILogger<McpRequestCorrelationService> logger)
    {
        this.sessionRegistry = sessionRegistry;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<JsonNode?> WaitForResponseAsync(string sessionId, string requestId, CancellationToken cancellationToken)
    {
        if (sessionRegistry is not McpSessionRegistry concreteRegistry)
        {
            throw new InvalidOperationException("Session registry is not the expected type.");
        }

        var state = concreteRegistry.TryGetSessionState(sessionId);
        if (state is null)
        {
            throw new KeyNotFoundException($"Session {sessionId} not found.");
        }

        if (!state.PendingRequests.TryGetValue(requestId, out var tcs))
        {
            throw new InvalidOperationException($"Pending request {requestId} not found in session {sessionId}.");
        }

        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(options.RequestTimeout);

            var response = await tcs.Task.WaitAsync(timeoutCts.Token).ConfigureAwait(false);
            state.CompletedRequests.TryAdd(requestId, 0);
            state.PendingRequests.TryRemove(requestId, out _);
            return response;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("Request {RequestId} in session {SessionId} timed out after {Timeout}", 
                requestId, sessionId, options.RequestTimeout);
            
            // Remove the pending request
            state.PendingRequests.TryRemove(requestId, out _);
            tcs.TrySetCanceled();
            
            // Return JSON-RPC timeout error
            return JsonNode.Parse($$"""
                {
                    "jsonrpc": "2.0",
                    "id": "{{requestId}}",
                    "error": {
                        "code": -32000,
                        "message": "Request timed out"
                    }
                }
                """);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error waiting for response to request {RequestId} in session {SessionId}", requestId, sessionId);
            state.PendingRequests.TryRemove(requestId, out _);
            throw;
        }
    }

    public void RegisterPendingRequest(string sessionId, string requestId, TaskCompletionSource<JsonNode> tcs)
    {
        if (sessionRegistry is not McpSessionRegistry concreteRegistry)
        {
            throw new InvalidOperationException("Session registry is not the expected type.");
        }

        var state = concreteRegistry.TryGetSessionState(sessionId);
        if (state is null)
        {
            throw new KeyNotFoundException($"Session {sessionId} not found.");
        }

        if (state.CompletedRequests.ContainsKey(requestId))
        {
            throw new InvalidOperationException($"Request {requestId} already completed in session {sessionId}.");
        }

        if (!state.PendingRequests.TryAdd(requestId, tcs))
        {
            throw new InvalidOperationException($"Request {requestId} already pending in session {sessionId}.");
        }

        if (state.EarlyResponses.TryRemove(requestId, out var earlyResponse))
        {
            tcs.TrySetResult(earlyResponse);
        }

        logger.LogDebug("Registered pending request {RequestId} in session {SessionId}", requestId, sessionId);
    }

    public void CompleteRequest(string sessionId, string requestId, JsonNode response)
    {
        if (sessionRegistry is not McpSessionRegistry concreteRegistry)
        {
            return;
        }

        var state = concreteRegistry.TryGetSessionState(sessionId);
        if (state is null)
        {
            logger.LogWarning("Session {SessionId} not found when completing request {RequestId}", sessionId, requestId);
            return;
        }

        if (state.PendingRequests.TryGetValue(requestId, out var tcs))
        {
            tcs.TrySetResult(response);
            logger.LogDebug("Completed request {RequestId} in session {SessionId}", requestId, sessionId);
        }
        else
        {
            state.EarlyResponses.TryAdd(requestId, response);
            logger.LogDebug("Request {RequestId} not found in pending requests for session {SessionId}, caching early response", requestId, sessionId);
        }
    }

    public void CancelRequest(string sessionId, string requestId)
    {
        if (sessionRegistry is not McpSessionRegistry concreteRegistry)
        {
            return;
        }

        var state = concreteRegistry.TryGetSessionState(sessionId);
        if (state is null)
        {
            return;
        }

        if (state.PendingRequests.TryRemove(requestId, out var tcs))
        {
            tcs.TrySetCanceled();
            logger.LogDebug("Cancelled request {RequestId} in session {SessionId}", requestId, sessionId);
        }
    }
}
