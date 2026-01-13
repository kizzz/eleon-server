namespace Eleon.McpGateway.Module.Domain;

public interface IMcpBackendRegistry
{
    IMcpBackend GetBackend(string name);

    IMcpBackend GetDefaultBackend();

    IReadOnlyCollection<IMcpBackend> GetAll();
}

