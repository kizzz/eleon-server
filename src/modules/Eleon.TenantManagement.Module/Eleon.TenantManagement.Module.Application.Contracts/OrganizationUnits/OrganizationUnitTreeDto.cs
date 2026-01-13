using System;
using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.OrganizationUnits
{
  public class OrganizationUnitTreeDto
  {
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string DisplayName { get; set; }
    public List<OrganizationUnitTreeDto> Children { get; set; }
  }
}
