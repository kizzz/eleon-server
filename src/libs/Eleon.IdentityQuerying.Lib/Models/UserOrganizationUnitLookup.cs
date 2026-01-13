using Volo.Abp.Identity;

namespace VPortal.TenantManagement.Module.OrganizationUnits
{
  public class UserOrganizationUnitLookup
  {
    public OrganizationUnit OrganizationUnit { get; set; }
    public bool IsMember { get; set; }
  }
}
