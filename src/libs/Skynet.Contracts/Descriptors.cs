using System.Text.Json.Serialization;

namespace Skynet.Contracts;

public sealed record class OperationDescriptor
{
    [JsonPropertyName("toolName")]
    public string ToolName { get; init; } = string.Empty;

    [JsonPropertyName("argumentsTemplate")]
    public string ArgumentsTemplate { get; init; } = string.Empty;

    [JsonPropertyName("argumentsSchemaRef")]
    public string? ArgumentsSchemaRef { get; init; }
}

public sealed record class WorkflowDescriptor
{
    [JsonPropertyName("workflowName")]
    public string WorkflowName { get; init; } = string.Empty;

    [JsonPropertyName("templateName")]
    public string? TemplateName { get; init; }

    [JsonPropertyName("inputTemplate")]
    public string InputTemplate { get; init; } = string.Empty;

    [JsonPropertyName("correlationKey")]
    public string? CorrelationKey { get; init; }
}

public sealed record class CodexExecutionDescriptor
{
    [JsonPropertyName("workspaceTemplate")]
    public string? WorkspaceTemplate { get; init; }

    [JsonPropertyName("missionFile")]
    public string? MissionFile { get; init; }

    [JsonPropertyName("missionContentTemplate")]
    public string MissionContentTemplate { get; init; } = string.Empty;

    [JsonPropertyName("timeoutSeconds")]
    public int TimeoutSeconds { get; init; }

    [JsonPropertyName("successCriteria")]
    public CodexSuccessCriteria SuccessCriteria { get; init; } = new();

    [JsonPropertyName("extraToolsProfile")]
    public string? ExtraToolsProfile { get; init; }
}

public sealed record class CodexSuccessCriteria
{
    [JsonPropertyName("type")]
    public CodexSuccessCriteriaType Type { get; init; }

    [JsonPropertyName("path")]
    public string? Path { get; init; }
}
