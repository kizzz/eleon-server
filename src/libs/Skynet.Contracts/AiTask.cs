using System;
using System.Text.Json.Serialization;

namespace Skynet.Contracts;

public sealed record class AiTask
{
    [JsonPropertyName("taskId")]
    public Guid TaskId { get; init; }

    [JsonPropertyName("tenantId")]
    public string TenantId { get; init; } = string.Empty;

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = string.Empty;

    [JsonPropertyName("goal")]
    public string Goal { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public TaskStatus Status { get; init; }

    [JsonPropertyName("currentStepPath")]
    public string? CurrentStepPath { get; init; }

    [JsonPropertyName("planVersion")]
    public int PlanVersion { get; init; }

    [JsonPropertyName("agentThreadId")]
    public string AgentThreadId { get; init; } = string.Empty;

    [JsonPropertyName("memorySessionId")]
    public string MemorySessionId { get; init; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; init; }
}
