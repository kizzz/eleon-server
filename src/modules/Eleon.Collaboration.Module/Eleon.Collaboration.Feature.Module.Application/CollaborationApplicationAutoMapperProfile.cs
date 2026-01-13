using AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using VPortal.Collaboration.Feature.Module.ChatInteractions;
using VPortal.Collaboration.Feature.Module.ChatMessages;
using VPortal.Collaboration.Feature.Module.ChatRooms;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.DocumentConversations;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module;

public class CollaborationApplicationAutoMapperProfile : Profile
{
  public CollaborationApplicationAutoMapperProfile()
  {
    CreateMap<ChatRoomEntity, ChatRoomDto>()
        .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.GetTags()))
        .ReverseMap()
        .Ignore(dest => dest.Tags)
        .Ignore(dest => dest.ViewChatPermissions)
        .AfterMap((dto, entity) => entity.SetTags(dto.Tags));
    CreateMap<ChatMessageEntity, ChatMessageDto>().ReverseMap();
    CreateMap<UserChatInfo, UserChatInfoDto>()
        .ReverseMap()
        .Ignore(x => x.AllowedRoles);
    CreateMap<DocumentConversationInfo, DocumentConversationInfoDto>().ReverseMap();

    CreateMap<OrganizationUnit, ChatOrganizationUnitDto>();
  }
}
