using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Notificator.Module.Entities
{
  public class NotificationEntity : AggregateRoot<Guid>, IHasCreationTime, IMultiTenant
  {
    public bool IsActive { get; set; }
    public Guid BackgroundJobId { get; set; }
    public string Receivers { get; set; }
    public string Message { get; set; }

    public string SerializedType { get; set; }

    public string TemplateName { get; set; }

    public NotificationPriority Priority { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }


    [Obsolete("EnironmentId is deprecated")]
    public string EnvironmentId { get; set; }

    public NotificationEntity()
    {
    }
    public NotificationEntity(Guid id)
    {
      Id = id;
    }

    private AbstractNotificationType? _type = null;
    [NotMapped]
    public AbstractNotificationType Type
    {
      get
      {
        if (_type != null)
        {
          return _type;
        }

        if (string.IsNullOrWhiteSpace(SerializedType))
        {
          return null;
        }

        _type = NotificationTypeHelper.FromJsonString(SerializedType);

        return _type;
      }

      set
      {
        _type = value;
        SerializedType = value.ToJsonString();
      }
    }

    [NotMapped]
    public Dictionary<string, string> DataParams
    {
      get
      {
        return ExtraProperties.ToDictionary(x => x.Key, x => x.Value?.ToString());
      }
      set
      {
        ExtraProperties.Clear();
        foreach (var item in value)
        {
          ExtraProperties[item.Key] = item.Value;
        }
      }
    }
  }
}
