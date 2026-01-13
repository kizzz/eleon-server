using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.LanguageManagement.Module.LocalizationOverrides;

namespace VPortal.LanguageManagement.Module.Controllers
{
  [Area(LanguageManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = LanguageManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/LanguageManagement/LocalizationOverride")]
  public class LocalizationOverrideController : LanguageManagementController, ILocalizationOverrideAppService
  {
    private readonly IVportalLogger<LocalizationOverrideController> logger;
    private readonly ILocalizationOverrideAppService appService;

    public LocalizationOverrideController(
        IVportalLogger<LocalizationOverrideController> logger,
        ILocalizationOverrideAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetLocalization")]
    public async Task<LocalizationDto> GetLocalization(GetLocalizationRequest getLocalizationRequest)
    {
      var response = await appService.GetLocalization(getLocalizationRequest);
      return response;
    }

    [HttpGet("GetLocalizationInformation")]
    public async Task<LocalizationInformationDto> GetLocalizationInformation()
    {
      var response = await appService.GetLocalizationInformation();
      return response;
    }

    [HttpPost("GetLocalizationStrings")]
    public async Task<PagedResultDto<OverriddenLocalizationStringDto>> GetLocalizationStrings(GetLocalizationStringsRequest request)
    {
      var response = await appService.GetLocalizationStrings(request);
      return response;
    }

    [HttpPost("OverrideLocalizationEntry")]
    public async Task<bool> OverrideLocalizationEntry(OverrideLocalizationEntryRequest request)
    {
      var response = await appService.OverrideLocalizationEntry(request);
      return response;
    }
  }
}
