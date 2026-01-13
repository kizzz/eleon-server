using Authorization.Module.RequestLocalization;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using VPortal.TenantManagement.Module.TenantLocalization;

namespace VPortal.LanguageManagement.Module.TenantLanguages
{
  public class TenantLocalizationAppService : LanguageManagementAppService, ITenantLocalizationAppService
  {
    private readonly RequestLanguageProvider requestLanguageProvider;
    private readonly IVportalLogger<TenantLocalizationAppService> logger;
    private readonly IObjectMapper objectMapper;

    public TenantLocalizationAppService(
        RequestLanguageProvider requestLanguageProvider,
        IVportalLogger<TenantLocalizationAppService> logger,
        IObjectMapper objectMapper)
    {
      this.requestLanguageProvider = requestLanguageProvider;
      this.logger = logger;
      this.objectMapper = objectMapper;
    }
    public async Task<LanguageEntryDto> GetTenantLanguage()
    {

      LanguageEntryDto result = null;
      try
      {
        var entity = await requestLanguageProvider.GetTenantLanguage();
        result = objectMapper.Map<LanguageCacheEntry, LanguageEntryDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetTenantLanguage(string cultureName, string uiCultureName)
    {

      try
      {
        await requestLanguageProvider.SetTenantLanguage(cultureName, uiCultureName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
