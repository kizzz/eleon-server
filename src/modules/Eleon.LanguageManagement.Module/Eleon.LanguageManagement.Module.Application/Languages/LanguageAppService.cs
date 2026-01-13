using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using VPortal.LanguageManagement.Module.Entities;
using VPortal.LanguageManagement.Module.Permissions;

namespace VPortal.LanguageManagement.Module.Languages
{
  public class LanguageAppService : LanguageManagementAppService, ILanguageAppService
  {
    private readonly IVportalLogger<LanguageAppService> logger;
    private readonly LanguageDomainService languageDomainService;

    public LanguageAppService(
        IVportalLogger<LanguageAppService> logger,
        LanguageDomainService languageDomainService)
    {
      this.logger = logger;
      this.languageDomainService = languageDomainService;
    }

    public async Task<LanguageInfoDto> GetDefaultLanguage()
    {
      LanguageInfoDto result = null;
      try
      {
        var lang = await languageDomainService.GetDefaultLanguage();
        result = ObjectMapper.Map<LanguageInfo, LanguageInfoDto>(lang);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(LanguageManagementPermissions.ManageLanguages)]
    public async Task<List<LanguageDto>> GetLanguageList()
    {
      List<LanguageDto> result = null;
      try
      {
        var languageEntities = await languageDomainService.GetLanguageList();
        result = ObjectMapper.Map<List<LanguageEntity>, List<LanguageDto>>(languageEntities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(LanguageManagementPermissions.ManageLanguages)]
    public async Task<bool> SetDefaultLanguage(Guid languageId)
    {
      bool result = false;
      try
      {
        await languageDomainService.SetDefaultLanguage(languageId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(LanguageManagementPermissions.ManageLanguages)]
    public async Task<bool> SetLanguageEnabled(SetLanguageEnabledDto request)
    {
      bool result = false;
      try
      {
        await languageDomainService.SetLanguageEnabled(request.LanguageId, request.IsEnabled);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
