using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.AppConfiguration;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using VPortal.TenantManagement.Module;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.HttpApi.Controllers;

[Area(TenantManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/TenantManagement/application-configuration/")]
public class ApplicationConfigurationController : TenantManagementController, IApplicationConfigurationAppService
{
  private readonly IVportalLogger<ApplicationConfigurationController> logger;
  private readonly IApplicationConfigurationAppService appService;

  public ApplicationConfigurationController(
      IVportalLogger<ApplicationConfigurationController> logger,
      IApplicationConfigurationAppService appService)
  {
    this.logger = logger;
    this.appService = appService;
  }

  [HttpGet("Get")]
  public async Task<ApplicationConfigurationDto> GetAsync(AppConfigRequestDto request)
  {

    try
    {
      return await appService.GetAsync(request);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return null;
  }
}
