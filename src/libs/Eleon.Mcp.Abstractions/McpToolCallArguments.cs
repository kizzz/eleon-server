using System.Text.Json;
using System.Text.Json.Nodes;

namespace Eleon.Mcp.Abstractions;

public sealed class McpToolCallArguments
{
    public static McpToolCallArguments Empty { get; } = new(new JsonObject());

    public McpToolCallArguments(JsonNode? value)
    {
        Value = value ?? new JsonObject();
    }

    public JsonNode Value { get; }

    public T? Deserialize<T>(JsonSerializerOptions? options = null) =>
        Value.Deserialize<T>(options ?? McpJsonSerializer.Default);
}
