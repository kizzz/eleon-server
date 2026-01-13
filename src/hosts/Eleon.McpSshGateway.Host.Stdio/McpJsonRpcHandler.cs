using System.Text.Json;
using System.Text.Json.Nodes;
using Eleon.JsonRpc.Stdio;
using Eleon.Mcp.Abstractions;
using Eleon.McpSshGateway.Module.Application.Mcp;
using Eleon.McpSshGateway.Module.Application.Contracts.Exceptions;
using Microsoft.Extensions.Logging;

namespace Eleon.McpSshGateway.Host.Stdio;

public sealed class McpJsonRpcHandler : IJsonRpcHandler
{
    private readonly McpService mcpService;
    private readonly ILogger<McpJsonRpcHandler> logger;

    public McpJsonRpcHandler(McpService mcpService, ILogger<McpJsonRpcHandler> logger)
    {
        this.mcpService = mcpService;
        this.logger = logger;
    }

    public async Task<JsonRpcResponse> HandleAsync(JsonRpcRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return request.Method switch
            {
                "initialize" => await HandleInitializeAsync(request, cancellationToken).ConfigureAwait(false),
                "tools/list" => await HandleListAsync(request, cancellationToken).ConfigureAwait(false),
                "tools/call" => await HandleCallAsync(request, cancellationToken).ConfigureAwait(false),
                _ => JsonRpcResponse.FromError(new JsonRpcError(JsonRpcErrorCodes.MethodNotFound, $"Method '{request.Method}' is not supported."), request.Id)
            };
        }
        catch (HostNotFoundException ex)
        {
            logger.LogWarning(ex, "Host not found");
            return JsonRpcResponse.FromError(CreateApplicationError(-32004, ex.Message), request.Id);
        }
        catch (HostDisabledException ex)
        {
            logger.LogWarning(ex, "Host disabled");
            return JsonRpcResponse.FromError(CreateApplicationError(-32005, ex.Message), request.Id);
        }
        catch (CommandRejectedException ex)
        {
            logger.LogWarning(ex, "Command rejected");
            return JsonRpcResponse.FromError(CreateApplicationError(-32006, ex.Message), request.Id);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid MCP request");
            return JsonRpcResponse.FromError(new JsonRpcError(JsonRpcErrorCodes.InvalidParams, ex.Message), request.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled MCP handler exception");
            return JsonRpcResponse.FromError(new JsonRpcError(JsonRpcErrorCodes.InternalError, "Internal error"), request.Id);
        }
    }

    private async Task<JsonRpcResponse> HandleInitializeAsync(JsonRpcRequest request, CancellationToken ct)
    {
        var parameters = DeserializeOrDefault<McpInitializeParams>(request.Params) ?? new McpInitializeParams("unknown", "0.0.0");
        var result = await mcpService.InitializeAsync(parameters, ct).ConfigureAwait(false);
        return JsonRpcResponse.Success(McpJsonSerializer.ToNode(result), request.Id);
    }

    private async Task<JsonRpcResponse> HandleListAsync(JsonRpcRequest request, CancellationToken ct)
    {
        var parameters = DeserializeOrDefault<ToolsListParams>(request.Params) ?? new ToolsListParams();
        var result = await mcpService.ListToolsAsync(parameters, ct).ConfigureAwait(false);
        return JsonRpcResponse.Success(McpJsonSerializer.ToNode(result), request.Id);
    }

    private async Task<JsonRpcResponse> HandleCallAsync(JsonRpcRequest request, CancellationToken ct)
    {
        var parameters = DeserializeOrDefault<ToolsCallParams>(request.Params) ?? new ToolsCallParams();
        if (string.IsNullOrWhiteSpace(parameters.Name))
        {
            throw new ArgumentException("The 'name' field is required for tools/call.");
        }

        var result = await mcpService.CallToolAsync(parameters, ct).ConfigureAwait(false);
        return JsonRpcResponse.Success(McpJsonSerializer.ToNode(result), request.Id);
    }

    private static T? DeserializeOrDefault<T>(JsonNode? node)
    {
        return node is null ? default : node.Deserialize<T>(McpJsonSerializer.Default);
    }

    private static JsonRpcError CreateApplicationError(int code, string message)
    {
        return new JsonRpcError(code, message);
    }
}
