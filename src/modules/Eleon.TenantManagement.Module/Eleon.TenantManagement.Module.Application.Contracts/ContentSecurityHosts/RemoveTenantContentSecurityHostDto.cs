using System;

namespace VPortal.TenantManagement.Module.ContentSecurityHosts
{
  public class RemoveTenantContentSecurityHostDto
  {
    public Guid? TenantId { get; set; }
    public Guid ContentSecurityHostId { get; set; }
  }
}
