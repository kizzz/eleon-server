using System.Text.Json.Nodes;

namespace Eleon.JsonRpc.Stdio;

public sealed class JsonRpcException : Exception
{
    public JsonRpcException(int code, string message, JsonNode? data = null, JsonNode? id = null)
        : base(message)
    {
        Code = code;
        DataNode = data;
        Id = id;
    }

    public int Code { get; }

    public JsonNode? DataNode { get; }

    public JsonNode? Id { get; }
}
