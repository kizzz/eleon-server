using Volo.Abp.MultiTenancy;

namespace VPortal.Notificator.Module.HubGroups
{
  public class AuditorTenantGroupId
  {
    private readonly ICurrentTenant tenant;

    public AuditorTenantGroupId(ICurrentTenant tenant)
    {
      this.tenant = tenant;
    }

    public override string ToString() => $"T-{tenant.Id?.ToString() ?? "HOST"}";
  }
}
