using System;

namespace VPortal.TenantManagement.Module.ContentSecurityHosts
{
  public class AddTenantContentSecurityHostDto
  {
    public Guid? TenantId { get; set; }
    public string Hostname { get; set; }
  }
}
