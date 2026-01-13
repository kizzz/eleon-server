using System;
using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.OrganizationUnits
{
  public class SetUserOrganizationUnitsInput
  {
    public Guid UserId { get; set; }
    public List<Guid> OrganizationUnitIds { get; set; }
  }
}
