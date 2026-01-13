using AutoMapper;
using Messaging.Module.ETO;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.ApplicationConfiguration.Module;

public class ApplicationConfigurationApplicationAutoMapperProfile : Profile
{
  public ApplicationConfigurationApplicationAutoMapperProfile()
  {
    CreateMap<OAuthConfigValueObject, OAuthConfigDto>();
    CreateMap<WebPushConfigValueObject, WebPushConfigDto>();
    CreateMap<ClientApplicationEto, ClientApplicationDto>();
    CreateMap<ApplicationModuleEto, ApplicationModuleDto>();
    CreateMap<ApplicationPropertyEto, ClientApplicationPropertyDto>();
    CreateMap<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>();
  }
}
