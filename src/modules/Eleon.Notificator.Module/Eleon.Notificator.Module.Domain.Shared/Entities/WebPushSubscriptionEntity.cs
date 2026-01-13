using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Notificator.Module.Entities
{
  public class WebPushSubscriptionEntity : CreationAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public WebPushSubscriptionEntity(Guid id, Guid userId, string endpoint, string p256dh, string auth)
    {
      Id = id;
      UserId = userId;
      Endpoint = endpoint;
      P256Dh = p256dh;
      Auth = auth;
    }

    protected WebPushSubscriptionEntity() { }

    public Guid UserId { get; protected set; }
    public string Endpoint { get; protected set; }
    public string P256Dh { get; protected set; }
    public string Auth { get; protected set; }
    public Guid? TenantId { get; set; }
  }
}
