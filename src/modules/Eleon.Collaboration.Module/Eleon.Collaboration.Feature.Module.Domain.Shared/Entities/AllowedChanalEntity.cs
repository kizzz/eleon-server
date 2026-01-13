using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using VPortal.Collaboration.Feature.Module.Entities;

namespace ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Entities;

[DisableAuditing]
public class ViewChatPermissionEntity : Entity<Guid>
{

  public Guid ChatId { get; set; }

  public ChatRoomEntity Chat { get; set; }

  public string EntityRef { get; set; }
  public PermissionEntityType EntityType { get; set; }

  public ViewChatPermissionEntity()
  {
  }

  public ViewChatPermissionEntity(Guid chatId) : base(chatId)
  {
  }
}
