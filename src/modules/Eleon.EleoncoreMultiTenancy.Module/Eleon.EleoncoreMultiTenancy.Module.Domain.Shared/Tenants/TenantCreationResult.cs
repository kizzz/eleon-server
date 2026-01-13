using System;

namespace VPortal.TenantManagement.Module.Tenants
{
  public class TenantCreationResult
  {
    public string TenantName { get; set; }
    public Guid? TenantId { get; set; }
    public string Error { get; set; }
    public bool Success { get; set; }
  }
}
