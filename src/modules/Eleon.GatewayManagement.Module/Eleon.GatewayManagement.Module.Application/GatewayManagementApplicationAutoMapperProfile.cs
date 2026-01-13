using AutoMapper;
using EventBusManagement.Module.EntityFrameworkCore;
using VPortal.GatewayManagement.Module.EventBuses;

namespace VPortal.GatewayManagement.Module;

public class GatewayManagementApplicationAutoMapperProfile : Profile
{
  public GatewayManagementApplicationAutoMapperProfile()
  {
    CreateMap<EventBusEntity, EventBusDto>().ReverseMap();
  }
}
