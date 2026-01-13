using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.LanguageManagement.Module.Languages;

namespace VPortal.LanguageManagement.Module.Controllers
{
  [Area(LanguageManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = LanguageManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/LanguageManagement/Language")]
  public class LanguageController : LanguageManagementController, ILanguageAppService
  {
    private readonly IVportalLogger<LanguageController> logger;
    private readonly ILanguageAppService appService;

    public LanguageController(
        IVportalLogger<LanguageController> logger,
        ILanguageAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetDefaultLanguage")]
    public async Task<LanguageInfoDto> GetDefaultLanguage()
    {
      var response = await appService.GetDefaultLanguage();
      return response;
    }

    [HttpGet("GetLanguageList")]
    public async Task<List<LanguageDto>> GetLanguageList()
    {
      var response = await appService.GetLanguageList();
      return response;
    }

    [HttpPost("SetDefaultLanguage")]
    public async Task<bool> SetDefaultLanguage(Guid languageId)
    {
      var response = await appService.SetDefaultLanguage(languageId);
      return response;
    }

    [HttpPost("SetLanguageEnabled")]
    public async Task<bool> SetLanguageEnabled(SetLanguageEnabledDto request)
    {
      var response = await appService.SetLanguageEnabled(request);
      return response;
    }
  }
}
