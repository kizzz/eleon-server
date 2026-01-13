using System.Text.Json;
using System.Text.Json.Nodes;

namespace Eleon.Mcp.Abstractions;

public sealed class McpToolCallResult
{
    public McpToolCallResult(JsonNode? value)
    {
        Value = value;
    }

    public JsonNode? Value { get; }

    public static McpToolCallResult From<T>(T payload, JsonSerializerOptions? options = null)
    {
        return new McpToolCallResult(JsonSerializer.SerializeToNode(payload, options ?? McpJsonSerializer.Default));
    }
}
