using Eleon.McpSshGateway.Domain.Entities;

namespace Eleon.McpSshGateway.Domain.Repositories;

public interface ICommandAuditRepository
{
    Task AddAsync(CommandAudit audit, CancellationToken cancellationToken = default);
}
