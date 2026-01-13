using Common.Module.Extensions;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.LanguageManagement.Module.LocalizationEntries;
using VPortal.LanguageManagement.Module.Permissions;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class LocalizationOverrideAppService : LanguageManagementAppService, ILocalizationOverrideAppService
  {
    private readonly IVportalLogger<LocalizationOverrideAppService> logger;
    private readonly LocalizationEntryDomainService localizationEntryDomainService;
    private readonly LocalizationOverrideDomainService localizationOverrideDomainService;

    public LocalizationOverrideAppService(
        IVportalLogger<LocalizationOverrideAppService> logger,
        LocalizationEntryDomainService localizationEntryDomainService,
        LocalizationOverrideDomainService localizationOverrideDomainService)
    {
      this.logger = logger;
      this.localizationEntryDomainService = localizationEntryDomainService;
      this.localizationOverrideDomainService = localizationOverrideDomainService;
    }

    public async Task<LocalizationDto> GetLocalization(GetLocalizationRequest request)
    {

      var result = new LocalizationDto();
      try
      {
        var strings = await localizationOverrideDomainService.GetLocalizationStrings(
        request.Culture,
        request.Culture,
        request.LocalizationResources ?? new List<string>(),
        string.Empty,
        0,
        int.MaxValue,
        $"{nameof(OverriddenLocalizationString.Key)} asc",
        false);

        var dtos = ObjectMapper.Map<List<OverriddenLocalizationString>, List<OverriddenLocalizationStringDto>>(strings.Value);

        result.Culture = request.Culture ?? "en";

        result.Resources = dtos.GroupBy(t =>
        {
          return t.Resource;
        }).Select(t => new LocalizationResourceDto()
        {
          ResourceName = t.Key,
          Texts = t.ToDictionary(r => r.Key, e => e.Target),
        }).ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<LocalizationInformationDto> GetLocalizationInformation()
    {
      LocalizationInformationDto result = null;
      try
      {
        var info = await localizationOverrideDomainService.GetLocalizationInformation();
        result = ObjectMapper.Map<LocalizationInformation, LocalizationInformationDto>(info);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(LanguageManagementPermissions.ManageLanguages)]
    public async Task<PagedResultDto<OverriddenLocalizationStringDto>> GetLocalizationStrings(GetLocalizationStringsRequest request)
    {
      PagedResultDto<OverriddenLocalizationStringDto> result = null;
      try
      {
        var strings = await localizationOverrideDomainService.GetLocalizationStrings(
            request.BaseCulture,
            request.TargetCulture,
            request.LocalizationResources,
            request.SearchQuery,
            request.SkipCount,
            request.MaxResultCount,
            request.Sorting.NonEmpty() ? request.Sorting : $"{nameof(OverriddenLocalizationString.Key)} asc",
            request.OnlyEmpty);

        var dtos = ObjectMapper.Map<List<OverriddenLocalizationString>, List<OverriddenLocalizationStringDto>>(strings.Value);

        result = new(strings.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(LanguageManagementPermissions.ManageLanguages)]
    public async Task<bool> OverrideLocalizationEntry(OverrideLocalizationEntryRequest request)
    {
      bool result = false;
      try
      {
        await localizationEntryDomainService.SetLocalizationEntry(
            request.CultureName,
            request.ResourceName,
            request.Key,
            request.NewValue);

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
