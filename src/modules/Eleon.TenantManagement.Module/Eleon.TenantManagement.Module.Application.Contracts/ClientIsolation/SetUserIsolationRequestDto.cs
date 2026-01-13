using System;

namespace VPortal.TenantManagement.Module.ClientIsolation
{
  public class SetUserIsolationRequestDto
  {
    public Guid UserId { get; set; }
    public bool Enabled { get; set; }
    public string ClientCertificateBase64 { get; set; }
    public string Password { get; set; }
  }
}
