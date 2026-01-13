using AutoMapper;
using EleoncoreProxy.Model;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using ProxyRouter.Minimal.HttpApi.Models.Constants;
using ProxyRouter.Minimal.HttpApi.Models.Messages;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;

namespace ProxyRouter.Minimal.Host.Profiles
{
  public class ModuleSettingsProfile : Profile
  {
    public ModuleSettingsProfile()
    {
      CreateMap<SitesManagementModuleSettingsDto, ModuleSettingsDto>();
      CreateMap<SitesManagementFullClientApplicationDto, ClientApplicationDto>();
      CreateMap<SitesManagementApplicationModuleDto, ApplicationModuleDto>();
      CreateMap<SitesManagementClientApplicationPropertyDto, ApplicationPropertyDto>();
      CreateMap<SitesManagementEleoncoreModuleDto, EleoncoreModuleDto>();

      CreateMap<ModuleCollectorLocation, ProxyRouter.Minimal.HttpApi.Models.Messages.Location>(MemberList.None)
          .Ignore(x => x.RemotePath)
          .Ignore(x => x.CheckedPath);
      CreateMap<ModuleCollectorLocationType, LocationType>();
    }
  }
}
