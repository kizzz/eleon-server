using System;
using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.OrganizationUnits
{
  public class SetRoleOrganizationUnitsInput
  {
    public string RoleName { get; set; }
    public List<Guid> OrganizationUnitIds { get; set; }
  }
}
