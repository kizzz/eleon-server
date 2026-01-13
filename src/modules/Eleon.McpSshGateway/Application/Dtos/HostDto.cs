namespace Eleon.McpSshGateway.Application.Dtos;

public sealed record HostDto(string Id, string Name, string HostName, int Port, string Username, IReadOnlyCollection<string> Tags);
