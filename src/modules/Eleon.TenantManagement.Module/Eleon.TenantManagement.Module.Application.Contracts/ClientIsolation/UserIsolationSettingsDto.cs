using System;

namespace VPortal.TenantManagement.Module.TenantIsolation
{
  public class UserIsolationSettingsDto
  {
    public Guid UserId { get; set; }
    public bool UserIsolationEnabled { get; set; }
    public string UserCertificateHash { get; set; }
    public bool TenantIsolationEnabled { get; set; }
  }
}
