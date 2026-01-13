using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using Volo.Abp;
using VPortal.TenantManagement.Module;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.HttpApi.Controllers;

[Area(TenantManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/tenant-management/application-configuration/")]
public class BaseApplicationConfigurationController : TenantManagementController, IBaseApplicationConfigurationAppService
{
  private readonly IVportalLogger<BaseApplicationConfigurationController> logger;
  private readonly IBaseApplicationConfigurationAppService appService;

  public BaseApplicationConfigurationController(
      IVportalLogger<BaseApplicationConfigurationController> logger,
      IBaseApplicationConfigurationAppService appService)
  {
    this.logger = logger;
    this.appService = appService;
  }

  [HttpGet("GetBase")]
  public async Task<EleoncoreApplicationConfigurationDto> GetBaseAsync()
  {

    try
    {
      return await appService.GetBaseAsync();
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }
}
