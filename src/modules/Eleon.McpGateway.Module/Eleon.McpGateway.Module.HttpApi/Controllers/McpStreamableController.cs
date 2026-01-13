using System.Text.Json;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Application.Contracts.Services;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.HttpApi.Controllers;

[ApiController]
[Route("mcp")]
public class McpStreamableController : ControllerBase
{
    private readonly IMcpGatewayDispatcher dispatcher;
    private readonly IMcpSessionRegistry sessionRegistry;
    private readonly McpRequestCorrelationService correlationService;
    private readonly McpStreamableOptions options;
    private readonly ILogger<McpStreamableController> logger;

    public McpStreamableController(
        IMcpGatewayDispatcher dispatcher,
        IMcpSessionRegistry sessionRegistry,
        McpRequestCorrelationService correlationService,
        IOptions<McpStreamableOptions> options,
        ILogger<McpStreamableController> logger)
    {
        this.dispatcher = dispatcher;
        this.sessionRegistry = sessionRegistry;
        this.correlationService = correlationService;
        this.options = options.Value;
        this.logger = logger;
    }

    [HttpPost("{backendName?}")]
    public async Task<IActionResult> PostAsync(
        string? backendName,
        CancellationToken cancellationToken)
    {
        // Extract session ID header (case-insensitive)
        var sessionIdHeader = Request.Headers.FirstOrDefault(h => 
            string.Equals(h.Key, "Mcp-Session-Id", StringComparison.OrdinalIgnoreCase));
        var sessionId = sessionIdHeader.Value.FirstOrDefault();

        // Parse JSON-RPC payload
        if (Request.ContentLength is 0)
        {
            return BadRequest("""{"error":"Missing JSON payload."}""");
        }

        JsonNode? payload;
        try
        {
            payload = await JsonNode.ParseAsync(Request.Body, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Invalid JSON payload received.");
            return BadRequest("""{"error":"Invalid JSON payload."}""");
        }

        if (payload is null)
        {
            return BadRequest("""{"error":"Missing JSON payload."}""");
        }

        // Resolve backend
        string backend;
        try
        {
            backend = dispatcher.ResolveBackendNameOrDefault(backendName);
            dispatcher.EnsureBackendExists(backend);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($$"""{"error":"Backend '{{backendName}}' not found."}""");
        }

        // Check if this is an initialize request
        var method = payload["method"]?.ToString();
        var isInitialize = string.Equals(method, "initialize", StringComparison.OrdinalIgnoreCase);

        // Handle session initialization
        if (isInitialize)
        {
            // Initialize always creates/uses session
            var sessionInfo = await sessionRegistry.GetOrCreateAsync(sessionId, backend, cancellationToken).ConfigureAwait(false);

            // Forward to backend
            await dispatcher.ForwardAsync(payload, backend, sessionInfo.SessionId, cancellationToken).ConfigureAwait(false);

            // Set session ID in response header
            Response.Headers["Mcp-Session-Id"] = sessionInfo.SessionId;

            // If request has id, wait for response
            var requestId = payload["id"]?.ToString();
            if (!string.IsNullOrWhiteSpace(requestId))
            {
                var tcs = new TaskCompletionSource<JsonNode>();
                try
                {
                    correlationService.RegisterPendingRequest(sessionInfo.SessionId, requestId, tcs);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogWarning(ex, "Duplicate pending request {RequestId} for session {SessionId}", requestId, sessionInfo.SessionId);
                    return Conflict("""{"error":"Request already pending."}""");
                }
                
                var response = await correlationService.WaitForResponseAsync(sessionInfo.SessionId, requestId, cancellationToken).ConfigureAwait(false);
                return Ok(response);
            }

            return Accepted();
        }

        // For non-initialize requests, session is required (unless tolerant mode)
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            if (options.TolerantMode)
            {
                // Auto-create session in tolerant mode
                var sessionInfo = await sessionRegistry.GetOrCreateAsync(null, backend, cancellationToken).ConfigureAwait(false);
                sessionId = sessionInfo.SessionId;
                Response.Headers["Mcp-Session-Id"] = sessionInfo.SessionId;
            }
            else
            {
                return BadRequest("""{"error":"Mcp-Session-Id header required"}""");
            }
        }
        else
        {
            // Validate session exists
            var sessionInfo = await sessionRegistry.TryGetAsync(sessionId, cancellationToken).ConfigureAwait(false);
            if (sessionInfo is null)
            {
                return BadRequest($$"""{"error":"Invalid or expired session: {{sessionId}}"}""");
            }
        }

        Response.Headers["Mcp-Session-Id"] = sessionId;

        // Forward to backend
        await dispatcher.ForwardAsync(payload, backend, sessionId, cancellationToken).ConfigureAwait(false);

        // If request has id, wait for response
        var id = payload["id"]?.ToString();
        if (!string.IsNullOrWhiteSpace(id))
        {
            var tcs = new TaskCompletionSource<JsonNode>();
            try
            {
                correlationService.RegisterPendingRequest(sessionId, id, tcs);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Duplicate pending request {RequestId} for session {SessionId}", id, sessionId);
                return Conflict("""{"error":"Request already pending."}""");
            }
            
            var response = await correlationService.WaitForResponseAsync(sessionId, id, cancellationToken).ConfigureAwait(false);
            return Ok(response);
        }

        // Notification (no id) - return 202 Accepted
        return Accepted();
    }

    [HttpGet("{backendName?}")]
    public async Task<IActionResult> GetAsync(
        string? backendName,
        CancellationToken cancellationToken)
    {
        // Extract session ID header (case-insensitive)
        var sessionIdHeader = Request.Headers.FirstOrDefault(h => 
            string.Equals(h.Key, "Mcp-Session-Id", StringComparison.OrdinalIgnoreCase));
        var sessionId = sessionIdHeader.Value.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return BadRequest("""{"error":"Mcp-Session-Id header required"}""");
        }

        // Resolve backend
        string backend;
        try
        {
            backend = dispatcher.ResolveBackendNameOrDefault(backendName);
            dispatcher.EnsureBackendExists(backend);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($$"""{"error":"Backend '{{backendName}}' not found."}""");
        }

        // Validate session exists
        var sessionInfo = await sessionRegistry.TryGetAsync(sessionId, cancellationToken).ConfigureAwait(false);
        if (sessionInfo is null)
        {
            return BadRequest($$"""{"error":"Invalid or expired session: {{sessionId}}"}""");
        }

        // Set up SSE response
        var response = Response;
        response.Headers.CacheControl = "no-store";
        response.Headers.Pragma = "no-cache";
        response.Headers.Connection = "keep-alive";
        response.Headers["X-Accel-Buffering"] = "no";
        response.Headers["Mcp-Session-Id"] = sessionId;
        response.ContentType = "text/event-stream";

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Request.HttpContext.RequestAborted);
        var heartbeatTask = SendHeartbeatsAsync(response, linkedCts.Token);

        try
        {
            await foreach (var message in dispatcher.GetOutboundStream(backend, sessionId, linkedCts.Token).WithCancellation(linkedCts.Token))
            {
                var payload = message.ToJsonString(serializerOptions);
                await response.WriteAsync("event: message\n", linkedCts.Token).ConfigureAwait(false);
                await response.WriteAsync($"data: {payload}\n\n", linkedCts.Token).ConfigureAwait(false);
                await response.Body.FlushAsync(linkedCts.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("SSE connection cancelled for session {SessionId}", sessionId);
        }
        finally
        {
            linkedCts.Cancel();
            await heartbeatTask.ConfigureAwait(false);
            linkedCts.Dispose();
        }

        return new EmptyResult();
    }

    [HttpDelete("{backendName?}")]
    public async Task<IActionResult> DeleteAsync(
        string? backendName,
        CancellationToken cancellationToken)
    {
        // Extract session ID header (case-insensitive)
        var sessionIdHeader = Request.Headers.FirstOrDefault(h => 
            string.Equals(h.Key, "Mcp-Session-Id", StringComparison.OrdinalIgnoreCase));
        var sessionId = sessionIdHeader.Value.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return BadRequest("""{"error":"Mcp-Session-Id header required"}""");
        }

        await sessionRegistry.TerminateAsync(sessionId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    private async Task SendHeartbeatsAsync(HttpResponse response, CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(options.SseKeepaliveInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                await response.WriteAsync(": keep-alive\n\n", cancellationToken).ConfigureAwait(false);
                await response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // connection closed
        }
    }
}
