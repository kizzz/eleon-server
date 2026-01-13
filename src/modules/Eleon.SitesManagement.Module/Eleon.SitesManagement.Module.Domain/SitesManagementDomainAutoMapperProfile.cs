using AutoMapper;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.DomainServices;
using Volo.Abp.AutoMapper;
using VPortal.SitesManagement.Module.CustomFeatures;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module;
public class SitesManagementDomainAutoMapperProfile : Profile
{
  public SitesManagementDomainAutoMapperProfile()
  {
    CreateMap<ApplicationModuleEntity, ApplicationModuleEto>();
    CreateMap<ModuleEntity, EleoncoreModuleEto>();
    CreateMap<ApplicationEntity, ClientApplicationEto>()
            .Ignore(c => c.Modules);
    CreateMap<ApplicationPropertyEntity, ApplicationPropertyEto>();
    CreateMap<ModuleSettingsGotMsg, ModuleSettingsRefreshedMsg>();
  }
}


