using System;

namespace VPortal.TenantManagement.Module.Tenants;

public class CreateDatabaseDto
{
  public Guid TenantId { get; set; }
  public string NewDatabaseName { get; set; }
  public string NewUserName { get; set; }
  public string NewUserPassword { get; set; }
}
