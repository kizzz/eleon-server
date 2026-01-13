using System.Text.Json.Nodes;

namespace Eleon.Mcp.Abstractions;

public sealed record ToolsListParams(int? Cursor = null);

public sealed record ToolsListResult(IReadOnlyList<McpToolDescriptor> Tools, string? NextCursor = null);

public sealed record ToolsCallParams
{
    public string Name { get; init; } = string.Empty;

    public JsonNode? Arguments { get; init; }
}

public sealed record ToolsCallResult(JsonNode? Result);
