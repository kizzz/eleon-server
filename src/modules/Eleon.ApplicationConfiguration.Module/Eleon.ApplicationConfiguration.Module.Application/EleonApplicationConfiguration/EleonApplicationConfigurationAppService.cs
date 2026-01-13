using Eleon.ApplicationConfiguration.Module.ApplicationConfigurations.Dtos;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System.Net;
using VPortal.ApplicationConfiguration.Module;
using VPortal.ApplicationConfiguration.Module.DomainServices;

namespace Eleon.ApplicationConfiguration.Module.ApplicationConfigurations;

public class EleonApplicationConfigurationAppService : ApplicationConfigurationModuleAppService, IApplicationConfigurationAppService
{
  private readonly IVportalLogger<EleonApplicationConfigurationAppService> _logger;
  private readonly ApplicationConfigurationDomainService _applicationConfigurationDomainService;

  public EleonApplicationConfigurationAppService(
    IVportalLogger<EleonApplicationConfigurationAppService> logger,
      ApplicationConfigurationDomainService applicationConfigurationDomainService)
  {
    _logger = logger;
    _applicationConfigurationDomainService = applicationConfigurationDomainService;
  }
  public async Task<EleoncoreApplicationConfigurationDto> GetAsync(ApplicationConfigurationRequestDto request)
  {

    try
    {
      var appId = WebUtility.UrlDecode(request.ApplicationIdentifier);

      var result = await _applicationConfigurationDomainService.GetAsync(appId);

      return ObjectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(result);
    }
    finally
    {
    }
  }
}
