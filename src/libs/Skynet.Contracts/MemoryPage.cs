using System;
using System.Text.Json.Serialization;

namespace Skynet.Contracts;

public sealed record class MemoryPage
{
    [JsonPropertyName("pageId")]
    public string PageId { get; init; } = string.Empty;

    [JsonPropertyName("sessionId")]
    public string SessionId { get; init; } = string.Empty;

    [JsonPropertyName("role")]
    public MemoryPageRole Role { get; init; }

    [JsonPropertyName("summary")]
    public string? Summary { get; init; }

    [JsonPropertyName("tokensApprox")]
    public int? TokensApprox { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("lastUsedAt")]
    public DateTimeOffset? LastUsedAt { get; init; }
}
