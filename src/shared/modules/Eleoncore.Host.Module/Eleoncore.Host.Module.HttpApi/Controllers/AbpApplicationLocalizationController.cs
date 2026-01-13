using Eleoncore.Host.Module.Eleoncore.Host.Module.HttpApi;
using global::Volo.Abp;
using global::Volo.Abp.AspNetCore.Mvc;
using global::Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace VPortal.Abp;

[Area(HostRemoteServiceConsts.ModuleName)]
[RemoteService(Name = HostRemoteServiceConsts.RemoteServiceName)]
[Route("api/abp/application-localization")]
public class AbpApplicationLocalizationController : AbpControllerBase, IAbpApplicationLocalizationAppService
{
  private readonly IAbpApplicationLocalizationAppService _localizationAppService;

  public AbpApplicationLocalizationController(IAbpApplicationLocalizationAppService localizationAppService)
  {
    _localizationAppService = localizationAppService;
  }

  [HttpGet]
  public virtual async Task<ApplicationLocalizationDto> GetAsync(ApplicationLocalizationRequestDto input)
  {
    return await _localizationAppService.GetAsync(input);
  }
}
