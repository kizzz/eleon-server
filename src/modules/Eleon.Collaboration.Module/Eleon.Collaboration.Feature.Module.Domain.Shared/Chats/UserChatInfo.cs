using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using Volo.Abp.Identity;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.Chats
{
  public class UserChatInfo
  {
    public int UnreadCount { get; set; }
    public ChatRoomEntity Chat { get; set; }
    public ChatMessageEntity LastChatMessage { get; set; }
    public bool IsChatMuted { get; set; }
    public bool IsArchived { get; set; }
    public ChatMemberRole UserRole { get; set; }
    public ChatMemberType MemberType { get; set; }
    public string MemberRef { get; set; }
    public List<OrganizationUnit> AllowedOrganizationUnits { get; set; }
    public List<string> AllowedRoles { get; set; }

  }
}
