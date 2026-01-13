using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.LanguageManagement.Module.Languages;
using VPortal.TenantManagement.Module.TenantLocalization;

namespace VPortal.LanguageManagement.Module.Controllers
{
  [Area(LanguageManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = LanguageManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/LanguageManagement/TenantLocalization")]
  public class TenantLocalizationController : LanguageManagementController, ITenantLocalizationAppService
  {
    private readonly IVportalLogger<TenantLocalizationController> logger;
    private readonly ITenantLocalizationAppService appService;

    public TenantLocalizationController(
        IVportalLogger<TenantLocalizationController> logger,
        ITenantLocalizationAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }


    [HttpGet("GetTenantLanguage")]
    public async Task<LanguageEntryDto> GetTenantLanguage()
    {
      var response = await appService.GetTenantLanguage();
      return response;
    }

    [HttpPost("SetTenantLanguage")]
    public async Task SetTenantLanguage(string cultureName, string uiCultureName)
    {
      await appService.SetTenantLanguage(cultureName, uiCultureName);
    }
  }
}
