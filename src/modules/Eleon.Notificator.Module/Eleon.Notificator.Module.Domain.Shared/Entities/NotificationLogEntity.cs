using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Notificator.Module.Entities
{
  public class NotificationLogEntity : AggregateRoot<Guid>, IHasCreationTime, IMultiTenant
  {
    public DateTime CreationTime { get; set; }

    public Guid? TenantId { get; set; }

    public NotificationPriority Priority { get; set; }

    public Guid? UserId { get; set; }

    public string Content { get; set; }

    public bool IsLocalizedData { get; set; }

    public string DataParams { get; set; }
    public string ApplicationName { get; set; }

    public bool IsRedirectEnabled { get; set; }
    public string RedirectUrl { get; set; }
    public bool IsRead { get; set; }


    public NotificationLogEntity()
    {
    }

    public NotificationLogEntity(
        Guid id,
        Guid? userId,
        string content,
        bool isLocalizedData,
        string dataParams,
        string applicationName,
        bool isRedirectEnabled,
        string redirectUrl,
        NotificationPriority priority)
    {
      Id = id;
      UserId = userId;
      Content = content;
      IsLocalizedData = isLocalizedData;
      DataParams = dataParams;
      IsRedirectEnabled = isRedirectEnabled;
      RedirectUrl = redirectUrl;
      Priority = priority;
      ApplicationName = applicationName;
    }
  }
}
