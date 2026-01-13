using System;

namespace VPortal.TenantManagement.Module.Tenants;

public class TenantConnectionStringDto
{
  public Guid TenantId { get; set; }

  public string Name { get; set; }

  public string Value { get; set; }
}
