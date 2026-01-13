using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Collaboration.Feature.Module.Entities
{
  [DisableAuditing]
  public class UserChatSettingEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid ChatRoomId { get; set; }
    public bool IsChatMuted { get; set; }
    public bool IsArchived { get; set; }

    public UserChatSettingEntity(Guid id, Guid userId, Guid chatId)
    {
      Id = id;
      UserId = userId;
      ChatRoomId = chatId;
    }

    protected UserChatSettingEntity() { }
  }
}
