using System;
using Volo.Abp.MultiTenancy;

namespace VPortal.TenantManagement.Module.OrganizationUnits
{
  public class CommonOrganizationUnitDto : IMultiTenant
  {
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Code { get; set; }

    public string DisplayName { get; set; }

    public Guid? TenantId { get; set; }
    public bool IsEnabled { get; set; }
  }
}
