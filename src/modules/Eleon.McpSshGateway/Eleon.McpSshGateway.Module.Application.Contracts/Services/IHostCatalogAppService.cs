using Eleon.McpSshGateway.Module.Application.Contracts.Dtos;
using Volo.Abp.Application.Services;

namespace Eleon.McpSshGateway.Module.Application.Contracts.Services;

public interface IHostCatalogAppService : IApplicationService
{
    Task<IReadOnlyList<HostDto>> ListAsync(CancellationToken cancellationToken);
    Task<HostDetailsDto?> GetAsync(string hostId, CancellationToken cancellationToken);
}

