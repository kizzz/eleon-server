using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skynet.Contracts;

public sealed record class AiPlan
{
    [JsonPropertyName("planId")]
    public Guid PlanId { get; init; }

    [JsonPropertyName("taskId")]
    public Guid TaskId { get; init; }

    [JsonPropertyName("version")]
    public int Version { get; init; }

    [JsonPropertyName("steps")]
    public IReadOnlyList<AiPlanStep> Steps { get; init; } = Array.Empty<AiPlanStep>();
}
