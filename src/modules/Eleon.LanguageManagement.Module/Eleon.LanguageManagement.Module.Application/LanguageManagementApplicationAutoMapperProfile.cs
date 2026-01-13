using Authorization.Module.RequestLocalization;
using AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.Localization;
using VPortal.LanguageManagement.Module.Entities;
using VPortal.LanguageManagement.Module.Languages;
using VPortal.LanguageManagement.Module.LocalizationOverrides;
using VPortal.TenantManagement.Module.TenantLocalization;

namespace VPortal.LanguageManagement.Module;

public class LanguageManagementApplicationAutoMapperProfile : Profile
{
  public LanguageManagementApplicationAutoMapperProfile()
  {
    CreateMap<LanguageEntity, LanguageDto>();
    CreateMap<LanguageInfo, LanguageInfoDto>()
        .Ignore(dest => dest.FlagIcon);

    CreateMap<OverriddenLocalizationString, OverriddenLocalizationStringDto>();
    CreateMap<LocalizationInformation, LocalizationInformationDto>();

    CreateMap<LanguageCacheEntry, LanguageEntryDto>();
  }
}
