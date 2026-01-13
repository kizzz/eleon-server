using Volo.Abp.MultiTenancy;

namespace BackgroundJobs.Module.HubGroups
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
