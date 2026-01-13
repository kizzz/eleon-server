using System.Threading.Tasks;
using TenantSettings.Module.Cache;

namespace VPortal.TenantManagement.Module.CorporateDomains
{
  public interface IDomainSettingsAppService
  {
    Task<List<TenantHostnameDto>> GetCurrentTenantHostnamesAsync();
    Task<List<TenantHostnameDto>> GetHostnamesByApplicationAsync(Guid? applicationId);
    Task<bool> AddCorporateDomainAsync(CreateCorporateDomainRequestDto request);
    Task<bool> UpdateCorporateDomainAsync(UpdateCorporateDomainRequestDto request);
    Task<bool> UpdateDomainApplicationAsync(Guid domainId, Guid? appId);
    Task<bool> RemoveCorporateDomainAsync(Guid id);

    Task<List<TenantHostnameDto>> GetHostnamesForTenantAsync(Guid? tenantId);
    Task<bool> AddCorporateDomainForTenantAsync(Guid? tenantId, CreateCorporateDomainRequestDto request);
    Task<bool> UpdateCorporateDomainForTenantAsync(Guid? tenantId, UpdateCorporateDomainRequestDto request);
    Task<bool> RemoveCorporateDomainForTenantAsync(Guid? tenantId, Guid domainId);
  }
}
