using AutoMapper;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Messages;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using Volo.Abp.AutoMapper;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.ApplicationConnectionStrings;
using VPortal.SitesManagement.Module.ApplicationMenuItems;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.CustomFeatures;
using VPortal.SitesManagement.Module.CustomPermissions;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module;

public class SitesManagementApplicationAutoMapperProfile : Profile
{
  public SitesManagementApplicationAutoMapperProfile()
  {
    //CreateMap<TenantConnectionString, TenantConnectionStringDto>()
    //    .ReverseMap();

    CreateMap<ApplicationTenantConnectionStringEntity, ConnectionStringDto>();

    CreateMap<ModuleEntity, EleoncoreModuleDto>()
        .ReverseMap();
    CreateMap<HealthCheckEntity, HealthCheckDto>().ReverseMap();

    CreateMap<ApplicationEntity, ClientApplicationDto>().ReverseMap();
    CreateMap<ApplicationEntity, FullClientApplicationDto>()
        .Ignore(c => c.Modules);
    CreateMap<ApplicationPropertyEntity, ClientApplicationPropertyDto>().ReverseMap();

    CreateMap<ApplicationModuleEntity, ApplicationModuleDto>().ReverseMap();
    CreateMap<ApplicationModuleEto, ApplicationModuleDto>();
    CreateMap<ClientApplicationEto, FullClientApplicationDto>(MemberList.None);
    CreateMap<EleoncoreModuleEto, EleoncoreModuleDto>()
        .ReverseMap();
    CreateMap<ModuleSettingsGotMsg, ModuleSettingsDto>();
    CreateMap<ApplicationMenuItemEntity, ApplicationMenuItemDto>().ReverseMap();

    CreateMap<OAuthConfigValueObject, OAuthConfigDto>();
    CreateMap<WebPushConfigValueObject, WebPushConfigDto>();
    CreateMap<ClientApplicationEto, ClientApplicationDto>();
    CreateMap<ApplicationModuleEto, ApplicationModuleDto>();
    CreateMap<ApplicationPropertyEto, ClientApplicationPropertyDto>();
    CreateMap<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>();

    //CreateMap<SiteEntity, SiteDto>()
    //    .Ignore(x => x.Hostnames)
    //    .ReverseMap()
    //    .ForMember(dest => dest.Hostnames, opt => opt.MapFrom(src => src.Hostnames.Select(x => x.Id)));
    //CreateMap<SiteSettingsValueObject, SiteSettingsDto>();

    CreateMap<CustomFeatureDto, FeatureDefinitionEto>().ReverseMap();
    CreateMap<CustomFeatureGroupDto, FeatureGroupDefinitionEto>().ReverseMap();
    CreateMap<CustomPermissionGroupDto, PermissionGroupDefinitionEto>().ReverseMap();
    CreateMap<CustomPermissionDto, PermissionDefinitionEto>().ReverseMap();
  }
}


