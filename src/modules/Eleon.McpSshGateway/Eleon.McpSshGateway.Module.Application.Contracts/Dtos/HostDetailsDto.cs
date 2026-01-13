namespace Eleon.McpSshGateway.Module.Application.Contracts.Dtos;

public sealed record HostDetailsDto(
    string Id,
    string Name,
    string HostName,
    int Port,
    string Username,
    IReadOnlyCollection<string> Tags,
    IReadOnlyCollection<string> AllowPatterns,
    IReadOnlyCollection<string> DenyPatterns,
    bool IsEnabled);

