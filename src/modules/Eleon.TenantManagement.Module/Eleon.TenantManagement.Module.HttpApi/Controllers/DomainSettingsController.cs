using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TenantSettings.Module.Cache;
using Volo.Abp;
using VPortal.TenantManagement.Module.CorporateDomains;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Authorize]
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/DomainSettings")]
  public class DomainSettingsController : TenantManagementController, IDomainSettingsAppService
  {
    private readonly IVportalLogger<DomainSettingsController> logger;
    private readonly IDomainSettingsAppService corporateDomainAppService;

    public DomainSettingsController(
        IVportalLogger<DomainSettingsController> logger,
        IDomainSettingsAppService corporateDomainAppService)
    {
      this.logger = logger;
      this.corporateDomainAppService = corporateDomainAppService;
    }

    [HttpGet("GetCurrentTenantHostnames")]
    public async Task<List<TenantHostnameDto>> GetCurrentTenantHostnamesAsync()
    {

      var response = await corporateDomainAppService.GetCurrentTenantHostnamesAsync();


      return response;
    }

    [HttpPost("AddCorporateDomain")]
    public async Task<bool> AddCorporateDomainAsync(CreateCorporateDomainRequestDto request)
    {

      var response = await corporateDomainAppService.AddCorporateDomainAsync(request);


      return response;
    }

    [HttpPost("UpdateCorporateDomain")]
    public async Task<bool> UpdateCorporateDomainAsync(UpdateCorporateDomainRequestDto request)
    {

      var response = await corporateDomainAppService.UpdateCorporateDomainAsync(request);


      return response;
    }

    [HttpPost("RemoveCorporateDomain")]
    public async Task<bool> RemoveCorporateDomainAsync(Guid id)
    {

      var response = await corporateDomainAppService.RemoveCorporateDomainAsync(id);


      return response;
    }

    [HttpGet("GetHostnamesForTenant")]
    public async Task<List<TenantHostnameDto>> GetHostnamesForTenantAsync(Guid? tenantId)
    {

      var response = await corporateDomainAppService.GetHostnamesForTenantAsync(tenantId);


      return response;
    }

    [HttpPost("AddCorporateDomainForTenant")]
    public async Task<bool> AddCorporateDomainForTenantAsync(Guid? tenantId, CreateCorporateDomainRequestDto request)
    {

      var response = await corporateDomainAppService.AddCorporateDomainForTenantAsync(tenantId, request);


      return response;
    }

    [HttpPost("UpdateCorporateDomainForTenant")]
    public async Task<bool> UpdateCorporateDomainForTenantAsync(Guid? tenantId, UpdateCorporateDomainRequestDto request)
    {

      var response = await corporateDomainAppService.UpdateCorporateDomainForTenantAsync(tenantId, request);


      return response;
    }

    [HttpPost("RemoveCorporateDomainForTenant")]
    public async Task<bool> RemoveCorporateDomainForTenantAsync(Guid? tenantId, Guid domainId)
    {

      var response = await corporateDomainAppService.RemoveCorporateDomainForTenantAsync(tenantId, domainId);


      return response;
    }

    [HttpGet("GetHostnamesByApplication")]
    public async Task<List<TenantHostnameDto>> GetHostnamesByApplicationAsync(Guid? applicationId)
    {

      var response = await corporateDomainAppService.GetHostnamesByApplicationAsync(applicationId);


      return response;
    }

    [HttpPost("UpdateDomainApplication")]
    public async Task<bool> UpdateDomainApplicationAsync(Guid domainId, Guid? appId)
    {

      var response = await corporateDomainAppService.UpdateDomainApplicationAsync(domainId, appId);


      return response;
    }
  }
}
