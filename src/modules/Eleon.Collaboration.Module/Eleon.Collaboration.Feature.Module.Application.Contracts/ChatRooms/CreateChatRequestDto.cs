

using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;

namespace VPortal.Collaboration.Feature.Module.ChatRooms
{
  public class CreateChatRequestDto
  {
    public string ChatName { get; set; }
    public Dictionary<Guid, ChatMemberRole?> InitialMembers { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; }
    public List<string> AllowedRoles { get; set; }
    public List<Guid> AllowedOrgUnits { get; set; }
    public ChatMemberRole DefaultRole { get; set; } = ChatMemberRole.Regular;
    public bool SetOwner { get; set; } = true;
  }
}
