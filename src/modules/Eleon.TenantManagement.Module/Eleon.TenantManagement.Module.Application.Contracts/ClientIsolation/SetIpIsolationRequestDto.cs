using System;
using System.Collections.Generic;
using TenantSettings.Module.Cache;

namespace VPortal.TenantManagement.Module.TenantIsolation
{
  public class SetIpIsolationRequestDto
  {
    public Guid? TenantId { get; set; }
    public bool IpIsolationEnabled { get; set; }
    public List<TenantWhitelistedIpDto> WhitelistedIps { get; set; }
  }
}
