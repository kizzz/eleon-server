using System.Text.Json.Nodes;

namespace Eleon.JsonRpc.Stdio;

public sealed record JsonRpcError(int Code, string Message, JsonNode? Data = null);
