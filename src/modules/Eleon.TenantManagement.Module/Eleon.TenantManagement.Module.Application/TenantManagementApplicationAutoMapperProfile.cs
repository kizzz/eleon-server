using AutoMapper;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using EleonsoftModuleCollector.Commons.Module.Messages.Identity;
using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.TenantSettings;
using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using Messaging.Module.ETO;
using ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys;
//using ModuleCollector.Identity.Module.Identity.Module.Domain.Shared.Entities;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using TenantSettings.Module.Cache;
using TenantSettings.Module.Models;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using VPortal.Identity.Module.Entities;
using VPortal.Identity.Module.Sessions;
using VPortal.Infrastructure.Module.Result;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.TenantManagement.Module.ControlDelegations;
using VPortal.TenantManagement.Module.CustomFeatures;
using VPortal.TenantManagement.Module.CustomPermissions;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.OrganizationUnits;
using VPortal.TenantManagement.Module.Roles;
using VPortal.TenantManagement.Module.Settings;
using VPortal.TenantManagement.Module.TenantAppearance;
using VPortal.TenantManagement.Module.TenantIsolation;
//using VPortal.TenantManagement.Module.Tenants;
using VPortal.TenantManagement.Module.TenantSettingsCache;
using VPortal.TenantManagement.Module.UserOtpSettings;
using VPortal.TenantManagement.Module.Users;
using VPortal.TenantManagement.Module.UserSettings;
using VPortal.TenantManagement.Module.ValueObjects;
using SharedModels = TenantSettings.Module.Models;

namespace VPortal.TenantManagement.Module;

public class TenantManagementApplicationAutoMapperProfile : Profile
{
  public TenantManagementApplicationAutoMapperProfile()
  {
    CreateMap<IdentityUser, CommonUserDto>()
        .Ignore(x => x.OrganizationUnits)
        .Ignore(x => x.Roles)
        .Ignore(x => x.LastLoginDate)
        .ForMember(x => x.ProfilePicture, opt => opt.MapFrom(x => x.GetProfilePicture()))
        .ForMember(x => x.ProfilePictureThumbnail, opt => opt.MapFrom(x => x.GetProfilePictureThumbnail()));
    //CreateMap<EleoncoreUser, CommonUserDto>()
    //    .Ignore(x => x.OrganizationUnits)
    //    .Ignore(x => x.Roles)
    //    .Ignore(x => x.LastLoginDate)
    //    .ForMember(x => x.ProfilePicture, opt => opt.MapFrom(x => x.GetProfilePicture()))
    //    .ForMember(x => x.ProfilePictureThumbnail, opt => opt.MapFrom(x => x.GetProfilePictureThumbnail()));

    CreateMap<CommonUserDto, IdentityUser>(MemberList.None);

    // Explicit mapping for IdentityUser to IdentityUserDto (required by AutoMapper 16.0.0 for nested mappings)
    CreateMap<IdentityUser, IdentityUserDto>();
    //CreateMap<CommonUserDto, EleoncoreUser>(MemberList.None);
    CreateMap<CommonRoleValueObject, CommonRoleDto>();
    CreateMap<OrganizationUnit, CommonOrganizationUnitDto>()
        .ForMember(x => x.IsEnabled, opt => opt.MapFrom(src => src.GetProperty("IsEnabled", false)))
        .ReverseMap();

    //CreateMap<TenantConnectionString, TenantConnectionStringDto>()
    //    .ReverseMap();

    //CreateMap<Tenant, CommonTenantDto>(MemberList.None)
    //    .ReverseMap();


    //CreateMap<Tenant, CommonTenantExtendedDto>()
    //    .Ignore(c => c.IsRoot)
    //    .Ignore(c => c.ParentId);
    CreateMap<UserSessionEto, UserSessionDto>().ReverseMap();

    CreateMap<IdentityApiKeyEto, IdentityApiKeyDto>().ReverseMap();

    CreateMap<ResultError, ResultErrorDto>();
    CreateMap<ResultSuccess, ResultSuccessDto>();

    CreateMap<TenantSettingEntity, TenantSettingDto>().ReverseMap();

    CreateMap<TenantExternalLoginProviderEntity, TenantExternalLoginProviderDto>().ReverseMap();

    CreateMap<TenantHostnameEntity, TenantHostnameDto>();

    CreateMap<ControlDelegationEntity, ControlDelegationDto>().ReverseMap();
    CreateMap<ControlDelegationHistoryEntity, ControlDelegationHistoryDto>().ReverseMap();

    CreateMap<UserIsolationSettingsEntity, UserIsolationSettingsDto>().ReverseMap();

    CreateMap<TenantWhitelistedIpEntity, TenantWhitelistedIpDto>().ReverseMap();

    CreateMap<UserOtpSettingsEntity, UserOtpSettingsDto>().ReverseMap();

    CreateMap<TenantContentSecurityHostEntity, TenantContentSecurityHostDto>().ReverseMap();

    CreateMap<TreeNodeValueObject<OrganizationUnit>, CommonOrganizationUnitTreeNodeDto>();

    CreateMap<UserRoleLookup, UserRoleLookupDto>();
    CreateMap<RoleUserLookup, RoleUserLookupDto>()
        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

    CreateMap<UserOrganizationUnitLookup, UserOrganizationUnitLookupDto>();


    CreateMap<IdentitySetting, IdentitySettingDto>().ReverseMap();

    CreateMap<AppearanceSettingsDto, AppearanceSettingsValueObject>().ReverseMap();
    CreateMap<ResultError, ResultErrorDto>();
    CreateMap<ResultSuccess, ResultSuccessDto>();
    CreateMap<ResultValueObject<AppearanceSettingsValueObject>, ResultDto<AppearanceSettingsDto>>().ReverseMap();

    CreateMap<TenantSettingEntity, SharedModels.TenantSetting>();
    CreateMap<TenantAppearanceSettingEntity, SharedModels.TenantAppearanceSetting>();
    CreateMap<TenantAppearanceSettingEntity, TenantAppearanceSettingDto>();
    CreateMap<TenantContentSecurityHostEntity, SharedModels.TenantContentSecurityHost>();
    CreateMap<TenantExternalLoginProviderEntity, SharedModels.TenantExternalLoginProvider>();
    CreateMap<TenantHostnameEntity, SharedModels.TenantHostnameValueObject>();
    CreateMap<TenantWhitelistedIpEntity, SharedModels.TenantWhitelistedIp>();
    CreateMap<UserIsolationSettingsEntity, SharedModels.UserIsolationSettings>();
    CreateMap<UserOtpSettingsEntity, SharedModels.UserOtpSettings>();

    CreateMap<TenantExternalLoginProvider, TenantExternalLoginProviderDto>();
    CreateMap<TenantAppearanceSetting, TenantAppearanceSettingDto>();
    CreateMap<TenantWhitelistedIp, TenantWhitelistedIpDto>();
    CreateMap<TenantContentSecurityHost, TenantContentSecurityHostDto>();
    CreateMap<TenantHostnameValueObject, TenantHostnameDto>();
    CreateMap<TenantSetting, TenantSettingDto>();
    CreateMap<UserIsolationSettings, UserIsolationSettingsDto>();
    CreateMap<TenantSettingsCacheValue, TenantSettingsCacheValueDto>();
    CreateMap<ImportExcelUsersValueObject, ImportExcelUsersValueObjectDto>();

    CreateMap<CustomPermissionGroupDto, PermissionGroupDefinitionRecord>()
        .Ignore(c => c.ExtraProperties)
        .ReverseMap()
        .Ignore(c => c.CategoryName)
        .ForMember(dest => dest.Dynamic, opt => opt.Ignore())
        .ForMember(dest => dest.Order, opt => opt.MapFrom(src =>
            src.ExtraProperties != null && src.ExtraProperties.ContainsKey("Order")
        ? int.Parse(src.ExtraProperties["Order"].ToString())
        : 0))
        .AfterMap((src, dest) =>
        {
          dest.Dynamic = src.ExtraProperties.TryGetValue("Dynamic", out var result) && result is bool boolResult && boolResult;
        });


    CreateMap<CustomPermissionDto, PermissionDefinitionRecord>()
        .ForMember(dest => dest.ExtraProperties, opt => opt.MapFrom((src, dest) =>
        {
          var extraProperties = dest.ExtraProperties ?? new Dictionary<string, object>();

          extraProperties["Order"] = src.Order;

          return extraProperties;
        }))
        .ReverseMap()
        .ForMember(dest => dest.Order, opt => opt.MapFrom(src =>
            src.ExtraProperties != null && src.ExtraProperties.ContainsKey("Order")
        ? int.Parse(src.ExtraProperties["Order"].ToString())
        : 0))
        .AfterMap((src, dest) =>
        {
          dest.Dynamic = src.ExtraProperties.TryGetValue("Dynamic", out var result) && result is bool boolResult && boolResult;
        });



    CreateMap<CustomFeatureGroupDto, FeatureGroupDefinitionRecord>()
        .Ignore(c => c.ExtraProperties)
        .ReverseMap()
        .ForMember(c => c.CategoryName, opt => opt.MapFrom(src => src.ExtraProperties.ContainsKey("CategoryName") ? src.ExtraProperties["CategoryName"].ToString() : string.Empty))
        .ForMember(c => c.IsDynamic, opt => opt.MapFrom(src => src.ExtraProperties.ContainsKey("Dynamic") ? src.ExtraProperties["Dynamic"] : false));

    CreateMap<CustomFeatureDto, FeatureDefinitionRecord>()
        .Ignore(c => c.ExtraProperties)
        .ReverseMap()
        .ForMember(c => c.IsDynamic, opt => opt.MapFrom(src => src.ExtraProperties.ContainsKey("Dynamic") ? src.ExtraProperties["Dynamic"] : false));

    CreateMap<UserSettingEntity, UserSettingDto>().ReverseMap();

    CreateMap<FeatureGroupDefinition, CustomFeatureGroupDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.Empty))
        .ForMember(dest => dest.IsDynamic, opt => opt.Ignore())
        .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
        .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
        .AfterMap((src, dest, context) =>
        {
          // dest.DisplayName = src.DisplayName

          dest.CategoryName = src.Properties.TryGetValue("CategoryName", out var category) ? category.ToString() : null;
          if (src.Properties.TryGetValue("Dynamic", out var dynamic))
          {
            if (dynamic is bool bd)
            {
              dest.IsDynamic = bd;
            }
            else
            {
              dest.IsDynamic = bool.TryParse(dynamic.ToString(), out var res) ? res : false;
            }
          }
          else
          {
            dest.IsDynamic = false;
          }
        });

    //CreateMap<FeatureDefinition, CustomFeatureDto>()
    //    .ForMember(dest => dest.Id, opt => opt.Ignore())
    //    .ForMember(dest => dest.IsDynamic, opt => opt.Ignore())
    //    .AfterMap((src, dest) =>
    //    {
    //        if (src.Properties.TryGetValue("Dynamic", out var dynamic))
    //        {
    //            if (dynamic is bool bd)
    //            {
    //                dest.IsDynamic = bd;
    //            }
    //            else
    //            {
    //                dest.IsDynamic = bool.TryParse(dynamic.ToString(), out var res) ? res : false;
    //            }
    //        }
    //        else
    //        {
    //            dest.IsDynamic = false;
    //        }
    //    });
    CreateMap<ResultValueObject<string>, ResultDto<string>>();

    CreateMap<ClientApplicationEto, ClientApplicationDto>();
    CreateMap<ApplicationModuleEto, ApplicationModuleDto>();
    CreateMap<ApplicationPropertyEto, ClientApplicationPropertyDto>();

    CreateMap<OAuthConfigValueObject, OAuthConfigDto>();
    CreateMap<WebPushConfigValueObject, WebPushConfigDto>();
    CreateMap<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>();

    //CreateMap<SiteEntity, SiteDto>()
    //    .Ignore(x => x.Hostnames)
    //    .ReverseMap()
    //    .ForMember(dest => dest.Hostnames, opt => opt.MapFrom(src => src.Hostnames.Select(x => x.Id)));
    //CreateMap<SiteSettingsValueObject, SiteSettingsDto>();

    CreateMap<TenantSystemHealthSettingsDto, TenantSystemHealthSettings>().ReverseMap();
    CreateMap<LoggingSettingsDto, LoggingSettings>().ReverseMap();
    CreateMap<TelemetrySettingsDto, TelemetrySettings>().ReverseMap();
  }
}
