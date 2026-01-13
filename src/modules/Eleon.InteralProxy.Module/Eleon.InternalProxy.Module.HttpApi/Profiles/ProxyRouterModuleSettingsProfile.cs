using AutoMapper;
using Common.Module.Constants;
using Eleon.InternalCommons.Lib.Messages.Locations;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Volo.Abp.AutoMapper;
using VPortal.SitesManagement.Module.ClientApplications;

namespace ProxyRouter.Minimal.Host.Profiles
{
  public class ProxyRouterModuleSettingsProfile : Profile
  {
    public ProxyRouterModuleSettingsProfile()
    {
      //CreateMap<VPortal.SitesManagement.Module.Microservices.EleoncoreModuleDto, EleoncoreModuleEto>()
      //    .ReverseMap();
      //CreateMap<ProxyRouter.Minimal.HttpApi.Models.Messages.EleoncoreModuleDto, EleoncoreModuleEto>()
      //    .ReverseMap();
      //CreateMap<VPortal.SitesManagement.Module.ClientApplications.ModuleSettingsDto, ModuleSettingsGotMsg>()
      //    .Ignore(c => c.TenantName);
      //CreateMap<FullClientApplicationDto, ClientApplicationEto>();
      //CreateMap<ClientApplicationPropertyDto, ApplicationPropertyEto>().ReverseMap();
      //CreateMap<VPortal.SitesManagement.Module.Microservices.ApplicationModuleDto, ApplicationModuleEto>();

      //CreateMap<ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations.Location, HttpApi.Models.Messages.Location>()
      //    .Ignore(x => x.RemotePath)
      //    .Ignore(x => x.CheckedPath)
      //    ;

      //CreateMap<LocationEto, HttpApi.Models.Messages.Location>()
      //          .Ignore(x => x.RemotePath)
      //          .Ignore(x => x.CheckedPath)
      //          ;

      //CreateMap<ProxyRouter.Minimal.HttpApi.Models.Messages.ModuleSettingsDto, Messaging.Module.Messages.ModuleSettingsGotMsg>()
      //    .Ignore(m => m.TenantName)
      //    .ReverseMap();
      //CreateMap<ProxyRouter.Minimal.HttpApi.Models.Messages.ClientApplicationDto, Messaging.Module.ETO.ClientApplicationEto>().ReverseMap();
      //CreateMap<ProxyRouter.Minimal.HttpApi.Models.Messages.ApplicationModuleDto, Messaging.Module.ETO.ApplicationModuleEto>()
      //    .ForMember(dest => dest.LoadLevel, opt => opt.MapFrom(src => (UiModuleLoadLevel)int.Parse(src.LoadLevel)))
      //    .ReverseMap()
      //    .ForMember(dest => dest.LoadLevel, opt => opt.MapFrom(src => ((int)src.LoadLevel).ToString()));
      //CreateMap<ProxyRouter.Minimal.HttpApi.Models.Messages.ApplicationPropertyDto, Messaging.Module.ETO.ApplicationPropertyEto>().ReverseMap();

      //CreateMap<Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations.ApplicationConfigurationDto, ProxyRouter.Minimal.HttpApi.Models.Messages.ApplicationConfigurationDto>();
    }
  }
}
