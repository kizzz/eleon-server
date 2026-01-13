using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using VPortal.Collaboration.Feature.Module.Chats;

namespace VPortal.Collaboration.Feature.Module.Entities
{
  [DisableAuditing]
  public class ChatMessageEntity : CreationAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public string Sender { get; set; }
    public ChatMessageSenderType SenderType { get; set; }
    public ChatMessageType Type { get; set; }
    public ChatMessageSeverity Severity { get; set; }
    public string Content { get; set; }

    public Guid ChatRoomId { get; set; }
    public ChatRoomEntity ChatRoom { get; set; }

    public ChatMessageEntity(Guid id)
    {
      Id = id;
    }

    #region
    [NotMapped]
    public bool Unread { get; set; }

    [NotMapped]
    public bool Outcoming { get; set; }

    [NotMapped]
    public ChatMemberInfo SenderInfo { get; set; }
    #endregion
  }
}
