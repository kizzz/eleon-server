using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement;
using VPortal.LanguageManagement.Module.Entities;
using VPortal.LanguageManagement.Module.Repositories;

namespace VPortal.LanguageManagement.Module.Languages
{
  public class LanguageDomainService : DomainService
  {
    private const string DefaultLanguageSettingKey = "Abp.Localization.DefaultLanguage";
    private const string EnabledLanguagesCacheKey = nameof(LanguageDomainService) + "_EnabledLanguages_";

    private readonly IVportalLogger<LanguageDomainService> logger;
    private readonly IMemoryCache memoryCache;
    private readonly ISettingManager settingManager;
    private readonly ILanguageRepository languageRepository;
    private readonly IDistributedEventBus massTransitPublisher;

    public LanguageDomainService(
        IVportalLogger<LanguageDomainService> logger,
        IMemoryCache memoryCache,
        ISettingManager settingManager,
        ILanguageRepository languageRepository,
        IDistributedEventBus massTransitPublisher)
    {
      this.logger = logger;
      this.memoryCache = memoryCache;
      this.settingManager = settingManager;
      this.languageRepository = languageRepository;
      this.massTransitPublisher = massTransitPublisher;
    }

    public async Task<List<LanguageEntity>> GetLanguageList()
    {
      List<LanguageEntity> result = null;
      try
      {
        result = await languageRepository.GetListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetLanguageEnabled(Guid languageId, bool enabled = true)
    {
      try
      {
        var lang = await languageRepository.GetAsync(languageId);
        if (lang.IsDefault && !enabled)
        {
          throw new Exception("Can not disable a default language.");
        }

        lang.IsEnabled = enabled;
        await languageRepository.UpdateAsync(lang);
        memoryCache.Remove(GetCacheKey(EnabledLanguagesCacheKey));
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task SetDefaultLanguage(Guid languageId)
    {
      try
      {
        var languages = await languageRepository.GetListAsync();

        var previousDefault = languages.FirstOrDefault(x => x.IsDefault);

        var newDefault = languages.First(x => x.Id == languageId);
        if (!newDefault.IsEnabled)
        {
          throw new Exception("Can not set a disabled language as default.");
        }

        newDefault.IsDefault = true;

        if (previousDefault != null && previousDefault.Id != newDefault.Id)
        {
          previousDefault.IsDefault = false;
          await languageRepository.UpdateAsync(previousDefault);
        }

        await languageRepository.UpdateAsync(newDefault);

        await settingManager.SetForCurrentTenantAsync(DefaultLanguageSettingKey, newDefault.CultureName, true);

        await NotifyDefaultLanguageChanged(newDefault);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<LanguageInfo> GetDefaultLanguage()
    {
      LanguageInfo result = null;
      try
      {
        var enabled = await GetEnabledLanguageEntities();
        result = enabled.FirstOrDefault(x => x.IsDefault)?.ToLanguageInfo();
        if (result == null)
        {
          result = new LanguageInfo("en", "en", "English");
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    internal async Task<List<LanguageInfo>> GetEnabledLanguages()
    {
      List<LanguageInfo> result = null;
      try
      {
        var enabled = await GetEnabledLanguageEntities();
        result = enabled.Select(x => x.ToLanguageInfo()).ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private async Task NotifyDefaultLanguageChanged(LanguageEntity newDefault)
    {
      var msg = new DefaultTenantLanguageUpdatedMsg
      {
        CultureName = newDefault.CultureName,
        UiCultureName = newDefault.UiCultureName
      };
      await massTransitPublisher.PublishAsync(msg);
    }

    private async Task<List<LanguageEntity>> GetEnabledLanguageEntities()
        => await memoryCache.GetOrCreateAsync(GetCacheKey(EnabledLanguagesCacheKey), async (entry) =>
        {
          var languages = await languageRepository.GetListAsync();
          return languages
                  .Where(x => x.IsEnabled)
                  .ToList();
        });

    private string GetCacheKey(string baseKey) => $"{baseKey}{CurrentTenant.Id.ToString() ?? "host"}";
  }
}
