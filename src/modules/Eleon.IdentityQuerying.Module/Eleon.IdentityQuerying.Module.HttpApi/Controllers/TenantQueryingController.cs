using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.HealthCheckModule.Module;
using AbpTenants = Volo.Abp.TenantManagement;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(IdentityQueryingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = IdentityQueryingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/identity-querying/tenants/")]
  public class TenantQueryingController : IdentityQueryingModuleController, ITenantAppService
  {
    private readonly ITenantAppService _tenantAppService;
    private readonly AbpTenants.ITenantAppService abpTenantAppService;

    public TenantQueryingController(
        ITenantAppService tenantAppService,
        AbpTenants.ITenantAppService abpTenantAppService)
    {
      this._tenantAppService = tenantAppService;
      this.abpTenantAppService = abpTenantAppService;
    }

    [HttpGet("GetCommonList")]
    public async Task<List<CommonTenantDto>> GetCommonTenantListAsync()
    {

      var response = await _tenantAppService.GetCommonTenantListAsync();


      return response;
    }

    [HttpGet("GetCommonTenant")]
    public async Task<CommonTenantDto> GetCommonTenant(Guid tenantId)
    {

      var response = await _tenantAppService.GetCommonTenant(tenantId);


      return response;
    }

    [HttpGet("GetDefaultConnectionStringAsync")]
    public async Task<string> GetDefaultConnectionStringAsync(Guid id)
    {

      var response = await abpTenantAppService.GetDefaultConnectionStringAsync(id);


      return response;
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
