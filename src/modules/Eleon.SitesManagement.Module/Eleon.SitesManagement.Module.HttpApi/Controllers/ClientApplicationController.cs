using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using System.Security.Policy;
using Volo.Abp;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.Microservices;
using Location = ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations.Location;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/CoreInfrastructure/ClientApplications")]
  public class ClientApplicationController : SitesManagementController, IClientApplicationAppService
  {
    private readonly IClientApplicationAppService _appService;
    private readonly IVportalLogger<ClientApplicationController> _logger;

    public ClientApplicationController(IClientApplicationAppService appService, IVportalLogger<ClientApplicationController> logger)
    {
      _appService = appService;
      _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ClientApplicationDto> GetAsync(Guid id)
    {

      var response = await _appService.GetAsync(id);


      return response;
    }

    [HttpGet("GetByTenantIdAsync")]
    public async Task<List<ClientApplicationDto>> GetByTenantIdAsync(Guid? tenantId)
    {

      var response = await _appService.GetByTenantIdAsync(tenantId);


      return response;
    }

    [HttpGet("GetAllAsync")]
    public async Task<List<FullClientApplicationDto>> GetAllAsync()
    {

      var response = await _appService.GetAllAsync();


      return response;
    }

    [HttpPost("CreateAsync")]
    public async Task<ClientApplicationDto> CreateAsync([FromBody] CreateClientApplicationDto input)
    {

      var response = await _appService.CreateAsync(input);


      return response;
    }

    [HttpPut("{id}")]
    public async Task<ClientApplicationDto> UpdateAsync(Guid id, [FromBody] UpdateClientApplicationDto input)
    {

      var response = await _appService.UpdateAsync(id, input);


      return response;
    }

    [HttpPut("{id}/UseDedicatedDatabase")]
    public async Task UseDedicatedDatabaseAsync(Guid id, bool useDedicatedDb)
    {

      await _appService.UseDedicatedDatabaseAsync(id, useDedicatedDb);

    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {

      await _appService.DeleteAsync(id);

    }

    [HttpGet("enabled-applications")]
    public async Task<List<ClientApplicationDto>> GetEnabledApplicationsAsync()
    {

      var response = await _appService.GetEnabledApplicationsAsync();


      return response;
    }

    [HttpPost("AddModuleToApplication")]
    public async Task<bool> AddModuleToApplication(ApplicationModuleDto addApplicationModuleDto)
    {

      var response = await _appService.AddModuleToApplication(addApplicationModuleDto);


      return response;
    }

    [HttpDelete("RemoveModuleToApplication")]
    public async Task<bool> RemoveModuleFromApplication(Guid applicationId, Guid moduleId)
    {

      var response = await _appService.RemoveModuleFromApplication(applicationId, moduleId);


      return response;
    }

    [HttpGet("GetSetting")]
    public async Task<ModuleSettingsDto> GetSetting()
    {

      var response = await _appService.GetSetting();


      return response;
    }

    [HttpPost("AddBulkModulesToApplication")]
    public async Task<bool> AddBulkModulesToApplication(List<ApplicationModuleDto> modules)
    {

      var response = await _appService.AddBulkModulesToApplication(modules);


      return response;
    }

    [HttpGet("GetLocations")]
    public async Task<List<Location>> GetLocationsAsync()
    {

      var response = await _appService.GetLocationsAsync();


      return response;
    }

    [HttpGet("GetDefaultApplication")]
    public async Task<ClientApplicationDto> GetDefaultApplicationAsync()
    {

      var response = await _appService.GetDefaultApplicationAsync();


      return response;
    }

    [HttpGet("GetLocationsBySiteId")]
    public async Task<List<Location>> GetLocationsBySiteIdAsync(Guid siteId)
    {

      var response = await _appService.GetLocationsBySiteIdAsync(siteId);


      return response;
    }

    [HttpGet("GetAppsSettingsBySiteId")]
    public async Task<ModuleSettingsDto> GetAppsSettingsBySiteIdAsync(Guid siteId)
    {

      var response = await _appService.GetAppsSettingsBySiteIdAsync(siteId);


      return response;
    }

    [HttpGet("GetSiteByHostname")]
    public async Task<ClientApplicationDto> GetSiteByHostnameAsync(string hostname)
    {

      var response = await _appService.GetSiteByHostnameAsync(hostname);


      return response;
    }
  }
}


