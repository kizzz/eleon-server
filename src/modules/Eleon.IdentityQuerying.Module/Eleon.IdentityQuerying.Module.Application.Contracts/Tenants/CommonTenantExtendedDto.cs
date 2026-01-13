using System;
using System.Collections.Generic;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants;

public class CommonTenantExtendedDto : CommonTenantDto
{
  public Guid? ParentId { get; set; }
  public new bool IsRoot { get; set; }
}
