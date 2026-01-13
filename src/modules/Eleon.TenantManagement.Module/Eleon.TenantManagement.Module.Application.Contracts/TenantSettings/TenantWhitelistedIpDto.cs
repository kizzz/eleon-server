using System;

namespace TenantSettings.Module.Cache
{
  public class TenantWhitelistedIpDto
  {
    public Guid? TenantId { get; set; }
    public string IpAddress { get; set; }
    public bool Enabled { get; set; }
  }
}
