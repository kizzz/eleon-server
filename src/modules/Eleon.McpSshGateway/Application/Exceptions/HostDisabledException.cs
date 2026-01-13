namespace Eleon.McpSshGateway.Application.Exceptions;

public sealed class HostDisabledException : Exception
{
    public HostDisabledException(string hostId)
        : base($"SSH host '{hostId}' is disabled.")
    {
        HostId = hostId;
    }

    public string HostId { get; }
}
