using AutoMapper;
using Messaging.Module.ETO;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module;

public class CollaborationDomainAutoMapperProfile : Profile
{
  public CollaborationDomainAutoMapperProfile()
  {
    CreateMap<ChatRoomEntity, ChatRoomEto>().ReverseMap();
    CreateMap<ChatMessageEntity, ChatMessageEto>().ReverseMap();
    CreateMap<ChatMemberInfo, ChatMemberInfoEto>().ReverseMap();
  }
}
