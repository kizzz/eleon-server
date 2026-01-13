using Eleon.McpSshGateway.Module.Domain.Entities;

namespace Eleon.McpSshGateway.Module.Domain.Repositories;

public interface ICommandAuditRepository
{
    Task AddAsync(CommandAudit audit, CancellationToken cancellationToken = default);
}

