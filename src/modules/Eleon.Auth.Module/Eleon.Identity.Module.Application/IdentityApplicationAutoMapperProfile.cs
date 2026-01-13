using AutoMapper;
using Messaging.Module.ETO;
// using ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys;
using Volo.Abp.Data;
using Volo.Abp.IdentityServer.Grants;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module;

public class IdentityApplicationAutoMapperProfile : Profile
{
  public IdentityApplicationAutoMapperProfile()
  {
    /* You can configure your AutoMapper mapping configuration here.
     * Alternatively, you can split your mapping configurations
     * into multiple profile classes for a better organization. */
    // CreateMap<ApiKeyEntity, IdentityApiKeyDto>();

    CreateMap<PersistedGrant, UserSessionEto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SessionId))
        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.SubjectId))
        .ForMember(dest => dest.SignInDate, opt => opt.MapFrom(src => src.CreationTime))
        .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Expiration))
        .ForMember(dest => dest.LastAccessTime, opt => opt.Ignore())
        .ForMember(dest => dest.Device, opt => opt.Ignore())
        .ForMember(dest => dest.IpAddress, opt => opt.Ignore())
        .ForMember(dest => dest.DeviceInfo, opt => opt.Ignore())
        .ForMember(dest => dest.Browser, opt => opt.Ignore())
        .AfterMap((src, dest) =>
        {
          try
          {
            dest.IpAddress = src.GetProperty("Ip")?.ToString();
            dest.Device = src.GetProperty("Device")?.ToString();
            dest.Browser = src.GetProperty("Browser")?.ToString();
            dest.DeviceInfo = src.GetProperty("DeviceInfo");
          }
          catch
          {

          }
        });
  }
}
