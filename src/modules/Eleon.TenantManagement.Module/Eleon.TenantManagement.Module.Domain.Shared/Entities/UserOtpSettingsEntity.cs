using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.TenantManagement.Module.Entities
{
  public class UserOtpSettingsEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public UserOtpSettingsEntity(Guid id)
    {
      Id = id;
    }

    protected UserOtpSettingsEntity() { }

    public Guid? TenantId { get; set; }
    public UserOtpType UserOtpType { get; set; }
    public string OtpEmail { get; set; }
    public string OtpPhoneNumber { get; set; }
    public Guid UserId { get; set; }
  }
}
