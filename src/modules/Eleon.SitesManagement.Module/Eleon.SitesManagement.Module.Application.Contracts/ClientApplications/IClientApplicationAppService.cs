using Volo.Abp.Application.Services;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using VPortal.SitesManagement.Module.Microservices;
using Location = ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations.Location;

namespace VPortal.SitesManagement.Module.ClientApplications
{
  public interface IClientApplicationAppService : IApplicationService
  {
    Task<ClientApplicationDto> GetAsync(Guid id);
    Task<ModuleSettingsDto> GetSetting();
    Task<List<ClientApplicationDto>> GetByTenantIdAsync(Guid? tenantId);
    Task<List<FullClientApplicationDto>> GetAllAsync();
    Task<bool> AddModuleToApplication(ApplicationModuleDto addApplicationModuleDto);
    Task<bool> RemoveModuleFromApplication(Guid applicationId, Guid moduleId);
    Task<ClientApplicationDto> CreateAsync(CreateClientApplicationDto input);
    Task<ClientApplicationDto> UpdateAsync(Guid id, UpdateClientApplicationDto input);
    Task UseDedicatedDatabaseAsync(Guid id, bool useDedicateDb);
    Task DeleteAsync(Guid id);
    Task<List<ClientApplicationDto>> GetEnabledApplicationsAsync();
    Task<bool> AddBulkModulesToApplication(List<ApplicationModuleDto> applicationModuleDtos);
    Task<List<Location>> GetLocationsAsync();

    Task<ClientApplicationDto> GetDefaultApplicationAsync();

    Task<List<Location>> GetLocationsBySiteIdAsync(Guid siteId);
    Task<ClientApplicationDto> GetSiteByHostnameAsync(string hostname);
    Task<ModuleSettingsDto> GetAppsSettingsBySiteIdAsync(Guid siteId);
  }
}


