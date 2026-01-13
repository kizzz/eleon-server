using Volo.Abp.MultiTenancy;

namespace VPortal.Notificator.Module.HubGroups
{
  public class TenantGroupId
  {
    private readonly ICurrentTenant tenant;

    public TenantGroupId(ICurrentTenant tenant)
    {
      this.tenant = tenant;
    }

    public override string ToString() => $"T-{tenant.Id?.ToString() ?? "HOST"}";
  }
}
