using System;
using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.Tenants;

public class CommonTenantExtendedDto : CommonTenantDto
{
  public Guid? ParentId { get; set; }
  public new bool IsRoot { get; set; }
}
