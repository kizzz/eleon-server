using System;

namespace VPortal.TenantManagement.Module.ContentSecurityHosts
{
  public class UpdateTenantContentSecurityHostDto
  {
    public Guid? TenantId { get; set; }
    public Guid ContentSecurityHostId { get; set; }
    public string NewHostname { get; set; }
  }
}
