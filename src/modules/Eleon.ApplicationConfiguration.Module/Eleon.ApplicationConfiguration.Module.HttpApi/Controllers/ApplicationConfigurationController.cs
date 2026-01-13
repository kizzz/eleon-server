using Eleon.ApplicationConfiguration.Module.ApplicationConfigurations;
using Eleon.ApplicationConfiguration.Module.ApplicationConfigurations.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Users;
using VPortal.ApplicationConfiguration.Module;

namespace Eleon.ApplicationConfiguration.Module.Controllers;

[Area(ApplicationConfigurationRemoteServiceConsts.ModuleName)]
[RemoteService(Name = ApplicationConfigurationRemoteServiceConsts.RemoteServiceName)]
[Route("api/application-configuration")]
public class ApplicationConfigurationController : ApplicationConfigurationModuleController, IApplicationConfigurationAppService
{
  private readonly IApplicationConfigurationAppService _appService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ICurrentUser _currentUser;

  public ApplicationConfigurationController(IApplicationConfigurationAppService appService, IHttpContextAccessor httpContextAccessor, ICurrentUser currentUser)
  {
    _appService = appService;
    _httpContextAccessor = httpContextAccessor;
    _currentUser = currentUser;
  }

  [HttpGet("get")]
  public async Task<EleoncoreApplicationConfigurationDto> GetAsync(ApplicationConfigurationRequestDto request)
  {
    return await _appService.GetAsync(request);
  }
}
