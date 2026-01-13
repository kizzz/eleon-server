using Eleon.McpSshGateway.Domain.Entities;

namespace Eleon.McpSshGateway.Domain.Repositories;

public interface IHostRepository
{
    Task<IReadOnlyList<SshHost>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SshHost?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
}
