using Volo.Abp.MultiTenancy;

namespace VPortal.Notificator.Module.HubGroups
{
  public class TenantAdminGroupId
  {
    private readonly ICurrentTenant tenant;

    public TenantAdminGroupId(ICurrentTenant tenant)
    {
      this.tenant = tenant;
    }

    public override string ToString() => $"T-Admins-{tenant.Id?.ToString() ?? "Admins-HOST"}";
  }
}
