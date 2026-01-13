using System;
using System.Collections.Generic;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants;

public class CommonTenantDto
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public int EntityVersion { get; set; }
  public bool IsRoot { get; set; }
  public List<TenantConnectionStringDto> ConnectionStrings { get; set; }
}
