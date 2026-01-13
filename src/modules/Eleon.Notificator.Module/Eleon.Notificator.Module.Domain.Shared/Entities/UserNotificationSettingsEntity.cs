using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Notificator.Module.Entities
{
  public class UserNotificationSettingsEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public UserNotificationSettingsEntity(
        Guid id,
        Guid userId,
        NotificationSourceType sourceType,
        bool sendNative = false,
        bool sendEmail = false)
    {
      Id = id;
      UserId = userId;
      SourceType = sourceType;
      SendNative = sendNative;
      SendEmail = sendEmail;
    }

    protected UserNotificationSettingsEntity()
    {
    }

    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public NotificationSourceType SourceType { get; set; }
    public bool SendNative { get; set; }
    public bool SendEmail { get; set; }
  }
}
