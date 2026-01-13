using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skynet.Contracts;

public sealed record class AiPlanStep
{
    [JsonPropertyName("stepId")]
    public string StepId { get; init; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("stepType")]
    public StepType StepType { get; init; }

    [JsonPropertyName("status")]
    public StepStatus Status { get; init; }

    [JsonPropertyName("attemptCount")]
    public int AttemptCount { get; init; }

    [JsonPropertyName("goalHash")]
    public string? GoalHash { get; init; }

    [JsonPropertyName("operation")]
    public OperationDescriptor? Operation { get; init; }

    [JsonPropertyName("workflow")]
    public WorkflowDescriptor? Workflow { get; init; }

    [JsonPropertyName("codex")]
    public CodexExecutionDescriptor? Codex { get; init; }

    [JsonPropertyName("substeps")]
    public IReadOnlyList<AiPlanStep> Substeps { get; init; } = Array.Empty<AiPlanStep>();
}
