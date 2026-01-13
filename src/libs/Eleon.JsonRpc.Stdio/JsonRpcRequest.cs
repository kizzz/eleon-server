using System.Text.Json.Nodes;

namespace Eleon.JsonRpc.Stdio;

public sealed record JsonRpcRequest(string Jsonrpc, string Method, JsonNode? Params, JsonNode? Id)
{
    public bool IsNotification => Id is null;
}
