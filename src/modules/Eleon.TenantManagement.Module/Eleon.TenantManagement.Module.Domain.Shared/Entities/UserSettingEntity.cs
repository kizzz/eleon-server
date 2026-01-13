using Common.Module.Constants;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.TenantManagement.Module.Entities
{
  public class UserSettingEntity : Entity<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public TwoFaNotificationType? TwoFaNotificationType { get; set; }

    public UserSettingEntity(Guid id)
    {
      Id = id;
    }

    protected UserSettingEntity() { }
  }
}
