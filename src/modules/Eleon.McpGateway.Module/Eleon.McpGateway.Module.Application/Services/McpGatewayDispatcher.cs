using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Application.Contracts.Services;
using Eleon.McpGateway.Module.Domain;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;

namespace Eleon.McpGateway.Module.Application.Services;

public class McpGatewayDispatcherAppService : ApplicationService, IMcpGatewayDispatcher
{
    private readonly IMcpBackendRegistry registry;
    private readonly IMcpSessionRegistry? sessionRegistry;
    private readonly ILogger<McpGatewayDispatcherAppService> logger;

    public McpGatewayDispatcherAppService(
        IMcpBackendRegistry registry,
        ILogger<McpGatewayDispatcherAppService> logger)
        : this(registry, null, logger)
    {
    }

    public McpGatewayDispatcherAppService(
        IMcpBackendRegistry registry,
        IMcpSessionRegistry? sessionRegistry,
        ILogger<McpGatewayDispatcherAppService> logger)
    {
        this.registry = registry;
        this.sessionRegistry = sessionRegistry;
        this.logger = logger;
    }

    public string ResolveBackendNameOrDefault(string? backendName)
    {
        if (string.IsNullOrWhiteSpace(backendName))
        {
            return registry.GetDefaultBackend().Name;
        }

        return backendName;
    }

    public void EnsureBackendExists(string backendName) => registry.GetBackend(backendName);

    public async Task ForwardAsync(JsonNode payload, string backendName, CancellationToken cancellationToken)
    {
        await ForwardAsync(payload, backendName, null, cancellationToken).ConfigureAwait(false);
    }

    public async Task ForwardAsync(JsonNode payload, string backendName, string? sessionId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(payload);

        IMcpBackend backend;
        if (!string.IsNullOrWhiteSpace(sessionId) && sessionRegistry is not null)
        {
            await sessionRegistry.GetOrCreateAsync(sessionId, backendName, cancellationToken).ConfigureAwait(false);
            await sessionRegistry.TouchAsync(sessionId, cancellationToken).ConfigureAwait(false);
            backend = await sessionRegistry.GetBackendAsync(sessionId, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            backend = registry.GetBackend(backendName);
        }

        var id = payload["id"]?.ToString();
        var method = payload["method"]?.ToString();
        logger.LogInformation("Forwarding JSON-RPC method {Method} (id: {Id}) to backend {Backend} (session: {SessionId}).", 
            method ?? "(unknown)", id ?? "(none)", backend.Name, sessionId ?? "(none)");
        var clonedPayload = payload.DeepClone() ?? payload;
        await backend.SendAsync(clonedPayload, cancellationToken).ConfigureAwait(false);
    }

    public IAsyncEnumerable<JsonNode> GetOutboundStream(string backendName, CancellationToken cancellationToken)
    {
        return GetOutboundStream(backendName, null, cancellationToken);
    }

    public async IAsyncEnumerable<JsonNode> GetOutboundStream(string backendName, string? sessionId, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IMcpBackend backend;
        if (!string.IsNullOrWhiteSpace(sessionId) && sessionRegistry is not null)
        {
            await sessionRegistry.GetOrCreateAsync(sessionId, backendName, cancellationToken).ConfigureAwait(false);
            await sessionRegistry.TouchAsync(sessionId, cancellationToken).ConfigureAwait(false);
            backend = await sessionRegistry.GetBackendAsync(sessionId, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            backend = registry.GetBackend(backendName);
        }

        await foreach (var message in backend.ReceiveAsync(cancellationToken).WithCancellation(cancellationToken))
        {
            yield return message;
        }
    }
}
