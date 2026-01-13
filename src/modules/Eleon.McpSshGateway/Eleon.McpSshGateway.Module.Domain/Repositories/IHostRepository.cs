using Eleon.McpSshGateway.Module.Domain.Entities;

namespace Eleon.McpSshGateway.Module.Domain.Repositories;

public interface IHostRepository
{
    Task<IReadOnlyList<SshHost>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SshHost?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
}

