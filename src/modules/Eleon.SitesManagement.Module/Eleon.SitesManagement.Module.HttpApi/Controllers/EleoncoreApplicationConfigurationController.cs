using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using Volo.Abp;
using VPortal.SitesManagement.Module;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.HttpApi.Controllers;

[Area(SitesManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/sites-management/eleoncore-application-configuration/")]
public class EleoncoreApplicationConfigurationController : SitesManagementController, IEleoncoreApplicationConfigurationAppService
{
  private readonly IVportalLogger<EleoncoreApplicationConfigurationController> logger;
  private readonly IEleoncoreApplicationConfigurationAppService appService;

  public EleoncoreApplicationConfigurationController(
      IVportalLogger<EleoncoreApplicationConfigurationController> logger,
      IEleoncoreApplicationConfigurationAppService appService)
  {
    this.logger = logger;
    this.appService = appService;
  }

  [HttpGet("Get")]
  public async Task<EleoncoreApplicationConfigurationDto> Get()
  {

    try
    {
      return await appService.Get();
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

  [HttpGet("GetByAppId")]
  public async Task<EleoncoreApplicationConfigurationDto> GetByAppIdAsync(string appId)
  {

    try
    {
      return await appService.GetByAppIdAsync(appId);
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


