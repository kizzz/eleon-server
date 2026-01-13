using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.JsonRpc.Stdio;

public sealed class JsonRpcStdioServer
{
    private readonly TextReader input;
    private readonly TextWriter output;
    private readonly ILogger<JsonRpcStdioServer> logger;
    private readonly JsonSerializerOptions serializerOptions;

    public JsonRpcStdioServer(
        TextReader? input = null,
        TextWriter? output = null,
        ILogger<JsonRpcStdioServer>? logger = null,
        JsonSerializerOptions? serializerOptions = null)
    {
        this.input = input ?? Console.In;
        this.output = output ?? Console.Out;
        this.logger = logger ?? NullLogger<JsonRpcStdioServer>.Instance;
        this.serializerOptions = serializerOptions ?? CreateSerializerOptions();
    }

    public async Task RunAsync(IJsonRpcHandler handler, CancellationToken cancellationToken)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            string? line;
            try
            {
                line = await input.ReadLineAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (line is null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            JsonRpcResponse? response = null;
            JsonNode? idForError = null;

            try
            {
                var request = ParseRequest(line);
                idForError = request.Id;

                var handlerResponse = await handler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

                if (request.IsNotification)
                {
                    continue;
                }

                response = handlerResponse ?? JsonRpcResponse.Success(null, request.Id);
                if (response.Id is null)
                {
                    response = response with { Id = request.Id };
                }
            }
            catch (JsonRpcException rpcEx)
            {
                logger.LogWarning(rpcEx, "JSON-RPC error while handling payload");
                response = JsonRpcResponse.FromError(
                    new JsonRpcError(rpcEx.Code, rpcEx.Message, rpcEx.DataNode),
                    rpcEx.Id ?? idForError);
            }
            catch (JsonException jsonEx)
            {
                logger.LogWarning(jsonEx, "Malformed JSON-RPC payload");
                response = JsonRpcResponse.FromError(
                    new JsonRpcError(JsonRpcErrorCodes.ParseError, "Parse error", JsonValue.Create(jsonEx.Message)),
                    null);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception while processing JSON-RPC message");
                response = JsonRpcResponse.FromError(
                    new JsonRpcError(JsonRpcErrorCodes.InternalError, "Internal error"),
                    idForError);
            }

            if (response is null)
            {
                continue;
            }

            if (response.Id is null && response.Error is null)
            {
                continue;
            }

            await WriteResponseAsync(response).ConfigureAwait(false);
        }
    }

    private static JsonSerializerOptions CreateSerializerOptions() => new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static JsonRpcRequest ParseRequest(string payload)
    {
        JsonNode? node;
        try
        {
            node = JsonNode.Parse(payload);
        }
        catch (JsonException ex)
        {
            throw new JsonRpcException(JsonRpcErrorCodes.ParseError, "Invalid JSON payload", JsonValue.Create(ex.Message));
        }

        if (node is not JsonObject obj)
        {
            throw new JsonRpcException(JsonRpcErrorCodes.InvalidRequest, "JSON-RPC request must be a JSON object");
        }

        if (!obj.TryGetPropertyValue("jsonrpc", out var versionNode) ||
            versionNode is not JsonValue versionValue ||
            !versionValue.TryGetValue(out string? version) ||
            !string.Equals(version, JsonRpcConstants.Version, StringComparison.Ordinal))
        {
            throw new JsonRpcException(JsonRpcErrorCodes.InvalidRequest, "jsonrpc must be the string '2.0'");
        }

        if (!obj.TryGetPropertyValue("method", out var methodNode) ||
            methodNode is not JsonValue methodValue ||
            !methodValue.TryGetValue(out string? method) ||
            string.IsNullOrWhiteSpace(method))
        {
            throw new JsonRpcException(JsonRpcErrorCodes.InvalidRequest, "method must be a non-empty string");
        }

        obj.TryGetPropertyValue("params", out var parameters);
        obj.TryGetPropertyValue("id", out var id);

        return new JsonRpcRequest(JsonRpcConstants.Version, method, parameters, id);
    }

    private async Task WriteResponseAsync(JsonRpcResponse response)
    {
        var json = JsonSerializer.Serialize(response, serializerOptions);
        await output.WriteLineAsync(json).ConfigureAwait(false);
        await output.FlushAsync().ConfigureAwait(false);
    }
}
