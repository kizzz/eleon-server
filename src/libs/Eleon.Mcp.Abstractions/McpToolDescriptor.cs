using System.Text.Json.Nodes;

namespace Eleon.Mcp.Abstractions;

public sealed record McpToolDescriptor
{
    public McpToolDescriptor(string name, string description, JsonObject? inputSchema = null)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("A tool name is required", nameof(name))
            : name;
        Description = description ?? string.Empty;
        InputSchema = inputSchema ?? CreateEmptySchema();
    }

    public string Name { get; }

    public string Description { get; }

    public JsonObject InputSchema { get; }

    private static JsonObject CreateEmptySchema()
    {
        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject(),
            ["required"] = new JsonArray()
        };
    }
}
