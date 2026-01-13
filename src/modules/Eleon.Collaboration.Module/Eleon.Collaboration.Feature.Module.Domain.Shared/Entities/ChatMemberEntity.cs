using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Collaboration.Feature.Module.Entities
{
  [DisableAuditing]
  public class ChatMemberEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public string RefId { get; set; }
    public Guid ChatRoomId { get; set; }
    public ChatMemberRole Role { get; set; }
    public ChatMemberType Type { get; set; } = ChatMemberType.User;
    public DateTime? LastViewedByUser { get; set; }

    public ChatMemberEntity(Guid id, string refId, Guid chatRoomId, ChatMemberRole role)
    {
      Id = id;
      RefId = refId;
      ChatRoomId = chatRoomId;
      Role = role;
    }
  }
}
