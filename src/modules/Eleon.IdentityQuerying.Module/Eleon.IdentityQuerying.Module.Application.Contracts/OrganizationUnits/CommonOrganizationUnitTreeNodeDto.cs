using System.Collections.Generic;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.OrganizationUnits
{
  public class CommonOrganizationUnitTreeNodeDto
  {
    public CommonOrganizationUnitDto Value { get; set; }
    public List<CommonOrganizationUnitTreeNodeDto> Children { get; set; }
    public CommonOrganizationUnitTreeNodeDto() { }
  }
}
