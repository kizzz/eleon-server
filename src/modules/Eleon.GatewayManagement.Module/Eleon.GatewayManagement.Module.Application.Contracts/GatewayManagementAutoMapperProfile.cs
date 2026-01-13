using AutoMapper;
using GatewayManagement.Module.Entities;
using GatewayManagement.Module.Proxies;
using VPortal.GatewayManagement.Module.Proxies;

namespace VPortal.GatewayManagement.Module
{
  public class GatewayManagementAutoMapperProfile : Profile
  {
    public GatewayManagementAutoMapperProfile()
    {
      CreateMap<GatewayEntity, GatewayDto>().ReverseMap();
      CreateMap<GatewayEntity, GatewayWorkspaceDto>(MemberList.None).ReverseMap();
      CreateMap<GatewayRegistrationKeyEntity, GatewayRegistrationKeyDto>();
    }
  }
}
