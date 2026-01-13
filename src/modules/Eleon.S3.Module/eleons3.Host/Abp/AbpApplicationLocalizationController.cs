using System.Threading.Tasks;
using global::Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using global::Volo.Abp.AspNetCore.Mvc;
using global::Volo.Abp;
using Microsoft.AspNetCore.Mvc;

namespace VPortal.Abp;

[Area("abp")]
[RemoteService(Name = "abp")]
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
