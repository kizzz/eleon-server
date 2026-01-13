using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.Tenants;
using AbpTenants = Volo.Abp.TenantManagement;
using VPortalTenants = VPortal.TenantManagement.Module.Tenants;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(EleoncoreMultiTenancyRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = EleoncoreMultiTenancyRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/Tenant/")]
  public class TenantController : EleoncoreMultiTenancyController, VPortalTenants.ITenantAppService, AbpTenants.ITenantAppService
  {
    private readonly IVportalLogger<TenantController> logger;
    private readonly VPortalTenants.ITenantAppService _tenantAppService;
    private readonly AbpTenants.ITenantAppService abpTenantAppService;

    public TenantController(
        IVportalLogger<TenantController> logger,
        VPortalTenants.ITenantAppService tenantAppService,
        AbpTenants.ITenantAppService abpTenantAppService)
    {
      this.logger = logger;
      this._tenantAppService = tenantAppService;
      this.abpTenantAppService = abpTenantAppService;
    }

    [HttpPost("CreateCommonTenant")]
    public async Task<VPortalTenants.TenantCreationResult> CreateCommonTenant(VPortalTenants.CreateTenantRequestDto request)
    {

      var response = await _tenantAppService.CreateCommonTenant(request);


      return response;
    }

    [HttpGet("GetCommonList")]
    public async Task<List<VPortalTenants.CommonTenantDto>> GetCommonTenantListAsync()
    {

      var response = await _tenantAppService.GetCommonTenantListAsync();


      return response;
    }

    [HttpGet("GetCommonTenant")]
    public async Task<VPortalTenants.CommonTenantDto> GetCommonTenant(Guid tenantId)
    {

      var response = await _tenantAppService.GetCommonTenant(tenantId);


      return response;
    }

    [HttpDelete("RemoveCommonTenant")]
    public async Task RemoveCommonTenant(VPortalTenants.CommonTenantDto tenantDto)
    {

      await _tenantAppService.RemoveCommonTenant(tenantDto);

    }

    [HttpPost("CreateDatabase")]
    public async Task<string> CreateDatabase(VPortalTenants.CreateDatabaseDto createDatabaseDto)
    {

      var response = await _tenantAppService.CreateDatabase(createDatabaseDto);


      return response;
    }

    [HttpGet("GetDefaultConnectionStringAsync")]
    public async Task<string> GetDefaultConnectionStringAsync(Guid id)
    {

      var response = await abpTenantAppService.GetDefaultConnectionStringAsync(id);


      return response;
    }

    [HttpPost("UpdateDefaultConnectionStringAsync")]
    public async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
    {

      await abpTenantAppService.UpdateDefaultConnectionStringAsync(id, defaultConnectionString);

    }

    [HttpPost("DeleteDefaultConnectionStringAsync")]
    public async Task DeleteDefaultConnectionStringAsync(Guid id)
    {

      await abpTenantAppService.DeleteDefaultConnectionStringAsync(id);

    }

    [HttpGet("GetAsync")]
    public async Task<AbpTenants.TenantDto> GetAsync(Guid id)
    {

      var response = await abpTenantAppService.GetAsync(id);

      return response;
    }

    [HttpGet("GetListAsync")]
    public async Task<PagedResultDto<AbpTenants.TenantDto>> GetListAsync(AbpTenants.GetTenantsInput input)
    {

      var response = await abpTenantAppService.GetListAsync(input);

      return response;
    }

    [HttpPost("CreateAsync")]
    public async Task<AbpTenants.TenantDto> CreateAsync(AbpTenants.TenantCreateDto input)
    {

      var response = await abpTenantAppService.CreateAsync(input);

      return response;
    }

    [HttpPost("UpdateAsync")]
    public async Task<AbpTenants.TenantDto> UpdateAsync(Guid id, AbpTenants.TenantUpdateDto input)
    {

      var response = await abpTenantAppService.UpdateAsync(id, input);

      return response;
    }

    [HttpPost("DeleteAsync")]
    public async Task DeleteAsync(Guid id)
    {

      await abpTenantAppService.DeleteAsync(id);

    }

    [HttpGet("GetCommonTenantExtendedListAsync")]
    public async Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListAsync()
    {

      var response = await _tenantAppService.GetCommonTenantExtendedListAsync();


      return response;
    }

    [HttpGet("GetCommonTenantExtendedListWithCurrent")]
    public async Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListWithCurrentAsync()
    {

      var response = await _tenantAppService.GetCommonTenantExtendedListWithCurrentAsync();


      return response;
    }
  }
}
