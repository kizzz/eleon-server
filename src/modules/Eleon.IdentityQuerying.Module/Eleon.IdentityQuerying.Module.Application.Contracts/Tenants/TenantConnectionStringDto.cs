using System;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants;

public class TenantConnectionStringDto
{
  public Guid TenantId { get; set; }

  public string Name { get; set; }

  public string Value { get; set; }
}
