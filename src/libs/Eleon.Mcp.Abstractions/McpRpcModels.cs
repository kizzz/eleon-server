using System.Text.Json.Nodes;

namespace Eleon.Mcp.Abstractions;

public sealed record McpRequest(string Method, JsonNode? Params, JsonNode? Id);

public sealed record McpResponse(JsonNode? Result, McpError? Error, JsonNode? Id)
{
    public static McpResponse FromError(McpError error, JsonNode? id = null) => new(null, error, id);
}

public sealed record McpError(int Code, string Message, JsonNode? Data = null);
