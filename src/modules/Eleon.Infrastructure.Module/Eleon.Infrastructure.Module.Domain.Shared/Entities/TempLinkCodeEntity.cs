using Newtonsoft.Json;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Infrastructure.Module.Entities
{
  public class TempLinkCodeEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Salt { get; set; }
    public string Parameters { get; set; }

    public TempLinkCodeEntity(Guid id, DateTime expiresAt, string salt, string parameters)
    {
      Id = id;
      ExpiresAt = expiresAt;
      Salt = salt;
      Parameters = parameters;
    }

    public string ToJson()
        => JsonConvert.SerializeObject(new
        {
          Id,
          TenantId,
          Salt,
          Parameters,
        });
  }
}
