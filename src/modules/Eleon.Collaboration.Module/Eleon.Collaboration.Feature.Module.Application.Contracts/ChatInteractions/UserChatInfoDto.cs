using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using Volo.Abp.Identity;
using VPortal.Collaboration.Feature.Module.ChatMessages;
using VPortal.Collaboration.Feature.Module.ChatRooms;

namespace VPortal.Collaboration.Feature.Module.ChatInteractions;

public class UserChatInfoDto
{
  public int UnreadCount { get; set; }
  public ChatRoomDto Chat { get; set; }
  public ChatMessageDto LastChatMessage { get; set; }
  public bool IsChatMuted { get; set; }
  public bool IsArchived { get; set; }
  public ChatMemberRole UserRole { get; set; }
  public ChatMemberType MemberType { get; set; }
  public string MemberRef { get; set; }
  public List<ChatOrganizationUnitDto> AllowedOrganizationUnits { get; set; }
  public List<string> AllowedRoles { get; set; }
}

public class ChatOrganizationUnitDto
{
  public Guid Id { get; set; }
  public string DisplayName { get; set; }
}
