namespace Eleon.McpSshGateway.Module.Application.Contracts.Exceptions;

public sealed class HostNotFoundException : Exception
{
    public HostNotFoundException(string hostId)
        : base($"SSH host '{hostId}' was not found.")
    {
        HostId = hostId;
    }

    public string HostId { get; }
}

