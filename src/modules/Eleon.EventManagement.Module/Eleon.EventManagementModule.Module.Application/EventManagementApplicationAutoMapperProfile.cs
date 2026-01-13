using AutoMapper;
using EventManagementModule.Module.Application.Contracts.Event;
using EventManagementModule.Module.Application.Contracts.Queue;
using EventManagementModule.Module.Application.Contracts.QueueDefenition;
using EventManagementModule.Module.Domain.Shared.Entities;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;


namespace VPortal.EventManagementModule.Module;

public class EventManagementApplicationAutoMapperProfile : Profile
{
  public EventManagementApplicationAutoMapperProfile()
  {
    CreateMap<EventEntity, EventDto>()
        .ForMember(dest => dest.Length, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Message) ? 0 : src.Message.Length));
    CreateMap<QueueEntity, QueueDto>()
        .Ignore(x => x.IsSystem);

    //CreateMap<QueueForwarderEntity, QueueDto>()
    //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Queue.Id))
    //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Queue.Name))
    //    .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.Queue.TenantId))
    //    .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Queue.Count))
    //    .ForMember(dest => dest.MessagesLimit, opt => opt.MapFrom(src => src.Queue.MessagesLimit))
    //    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Queue.Type.ToString()))
    //    ;

    CreateMap<EventEntity, FullEventDto>()
      .Ignore(dest => dest.Token)
      .Ignore(dest => dest.Claims)
      .AfterMap((src, dest) =>
      {
        if (src.ExtraProperties == null) return;

        dest.Token = src.GetProperty("Token")?.ToString();
        dest.Claims = src.GetProperty("Claims")?.ToString();
      })
      .ReverseMap()
      .Ignore(x => x.ExtraProperties);
    //CreateMap<QueueEntity, FullQueueDto>()
    //    .ReverseMap();

    //CreateMap<ForwarderEntity, ForwarderDto>()
    //    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
    //    .ReverseMap()
    //    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<EventSourceType>(src.Type)));

    CreateMap<QueueDefinitionEntity, QueueDefinitionDto>();
  }
}
