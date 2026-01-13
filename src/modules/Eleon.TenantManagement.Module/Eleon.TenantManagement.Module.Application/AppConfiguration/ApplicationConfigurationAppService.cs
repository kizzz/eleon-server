using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.AppConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using VPortal.TenantManagement.Module;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.AppConfiguration;
public class ApplicationConfigurationAppService : TenantManagementAppService, IApplicationConfigurationAppService
{
  private readonly IVportalLogger<ApplicationConfigurationAppService> _logger;
  private readonly AbpApplicationConfigurationAppService _abpAppService;

  public ApplicationConfigurationAppService(
      IVportalLogger<ApplicationConfigurationAppService> logger,
      AbpApplicationConfigurationAppService abpAppService)
  {
    _logger = logger;
    _abpAppService = abpAppService;
  }

  public async Task<ApplicationConfigurationDto> GetAsync(AppConfigRequestDto request)
  {

    try
    {
      return await _abpAppService.GetAsync(new ApplicationConfigurationRequestOptions
      {
        IncludeLocalizationResources = request.IncludeLocalizationResources,
      });
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }

    return null;
  }
}
