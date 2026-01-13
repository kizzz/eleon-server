using Eleon.McpSshGateway.Module.Application.Contracts.Dtos;
using Eleon.McpSshGateway.Module.Application.Contracts.Services;
using Eleon.McpSshGateway.Module.Domain.Entities;
using Eleon.McpSshGateway.Module.Domain.Repositories;
using Volo.Abp.Application.Services;

namespace Eleon.McpSshGateway.Module.Application.Services;

public sealed class HostCatalogAppService : ApplicationService, IHostCatalogAppService
{
    private readonly IHostRepository hostRepository;

    public HostCatalogAppService(IHostRepository hostRepository)
    {
        this.hostRepository = hostRepository;
    }

    public async Task<IReadOnlyList<HostDto>> ListAsync(CancellationToken cancellationToken)
    {
        var hosts = await hostRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return hosts
            .OrderBy(h => h.Name, StringComparer.OrdinalIgnoreCase)
            .Select(MapHost)
            .ToArray();
    }

    public async Task<HostDetailsDto?> GetAsync(string hostId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(hostId))
        {
            return null;
        }

        var host = await hostRepository.FindByIdAsync(hostId, cancellationToken).ConfigureAwait(false);
        return host is null ? null : MapDetails(host);
    }

    private static HostDto MapHost(SshHost host) =>
        new(host.Id, host.Name, host.HostName, host.Port, host.Username, host.Tags);

    private static HostDetailsDto MapDetails(SshHost host) =>
        new(host.Id, host.Name, host.HostName, host.Port, host.Username, host.Tags, host.AllowPatterns, host.DenyPatterns, host.IsEnabled);
}

