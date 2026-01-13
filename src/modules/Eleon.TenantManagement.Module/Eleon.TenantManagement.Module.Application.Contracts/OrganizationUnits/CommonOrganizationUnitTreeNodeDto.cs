using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.OrganizationUnits
{
  public class CommonOrganizationUnitTreeNodeDto
  {
    public CommonOrganizationUnitDto Value { get; set; }
    public List<CommonOrganizationUnitTreeNodeDto> Children { get; set; }
    public CommonOrganizationUnitTreeNodeDto() { }
  }
}
