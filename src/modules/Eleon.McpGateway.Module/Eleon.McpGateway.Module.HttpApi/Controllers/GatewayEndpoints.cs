using System.Text.Json;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Application.Contracts.Services;
using Eleon.McpGateway.Module.Infrastructure;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.HttpApi.Controllers;

[ApiController]
public class GatewayEndpoints : ControllerBase
{
    private readonly IMcpGatewayDispatcher dispatcher;
    private readonly SseConnectionCoordinator coordinator;
    private readonly IOptions<McpGatewayOptions> options;
    private readonly ILogger<GatewayEndpoints> logger;

    public GatewayEndpoints(
        IMcpGatewayDispatcher dispatcher,
        SseConnectionCoordinator coordinator,
        IOptions<McpGatewayOptions> options,
        ILogger<GatewayEndpoints> logger)
    {
        this.dispatcher = dispatcher;
        this.coordinator = coordinator;
        this.options = options;
        this.logger = logger;
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Content("""{"status":"ok"}""", "application/json");
    }

    [HttpGet("{basePath}/{backendName?}")]
    public async Task<IActionResult> StreamSseAsync(
        string basePath,
        string? backendName,
        CancellationToken cancellationToken)
    {
        var gatewayOptions = options.Value;
        if (basePath != gatewayOptions.BasePath.TrimStart('/'))
        {
            return NotFound();
        }

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
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

        var response = Response;
        response.Headers.CacheControl = "no-store";
        response.Headers.Pragma = "no-cache";
        response.Headers.Connection = "keep-alive";
        response.Headers["X-Accel-Buffering"] = "no";
        response.ContentType = "text/event-stream";

        var sessionId = Request.Headers["Mcp-Session-Id"].FirstOrDefault();
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Request.HttpContext.RequestAborted);
        var heartbeatTask = SendHeartbeatsAsync(response, linkedCts.Token);

        try
        {
            await foreach (var message in dispatcher.GetOutboundStream(backend, sessionId, linkedCts.Token).WithCancellation(linkedCts.Token))
            {
                var payload = message.ToJsonString(serializerOptions);
                await response.WriteAsync("event: message\n", linkedCts.Token);
                await response.WriteAsync($"data: {payload}\n\n", linkedCts.Token);
                await response.Body.FlushAsync(linkedCts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("SSE connection cancelled.");
        }
        finally
        {
            linkedCts.Cancel();
            await heartbeatTask.ConfigureAwait(false);
            linkedCts.Dispose();
        }

        return new EmptyResult();
    }

    private static async Task SendHeartbeatsAsync(HttpResponse response, CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
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

    [HttpPost("{basePath}/{backendName?}")]
    public async Task<IActionResult> AcceptMessageAsync(
        string basePath,
        string? backendName,
        CancellationToken cancellationToken)
    {
        var gatewayOptions = options.Value;
        if (basePath != gatewayOptions.BasePath.TrimStart('/'))
        {
            return NotFound();
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

        var sessionId = Request.Headers["Mcp-Session-Id"].FirstOrDefault();
        await dispatcher.ForwardAsync(payload, backend, sessionId, cancellationToken).ConfigureAwait(false);
        return Accepted();
    }

    [HttpOptions("{basePath}/{backendName?}")]
    public IActionResult HandleOptions(string basePath, string? backendName)
    {
        var gatewayOptions = options.Value;
        if (basePath != gatewayOptions.BasePath.TrimStart('/'))
        {
            return NotFound();
        }

        return Ok();
    }
}

