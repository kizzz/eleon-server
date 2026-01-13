using System.Text.Json.Nodes;

namespace Eleon.JsonRpc.Stdio;

public sealed record JsonRpcResponse(string Jsonrpc, JsonNode? Result, JsonRpcError? Error, JsonNode? Id)
{
    public static JsonRpcResponse Success(JsonNode? result, JsonNode? id) => new(JsonRpcConstants.Version, result, null, id);

    public static JsonRpcResponse FromError(JsonRpcError error, JsonNode? id) => new(JsonRpcConstants.Version, null, error, id);

    public bool IsNotification => Id is null;
}
