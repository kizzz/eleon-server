using System;
using System.Text.Json.Serialization;

namespace Skynet.Contracts;

public sealed record class MemorySession
{
    [JsonPropertyName("sessionId")]
    public string SessionId { get; init; } = string.Empty;

    [JsonPropertyName("tenantId")]
    public string TenantId { get; init; } = string.Empty;

    [JsonPropertyName("taskId")]
    public Guid TaskId { get; init; }

    [JsonPropertyName("agentThreadId")]
    public string AgentThreadId { get; init; } = string.Empty;

    [JsonPropertyName("activePageId")]
    public string? ActivePageId { get; init; }
}
