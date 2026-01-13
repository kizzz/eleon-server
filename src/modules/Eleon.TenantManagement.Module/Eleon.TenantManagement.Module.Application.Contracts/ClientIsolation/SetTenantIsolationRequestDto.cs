using System;

namespace VPortal.TenantManagement.Module.ClientIsolation
{
  public class SetTenantIsolationRequestDto
  {
    public Guid? TenantId { get; set; }
    public bool Enabled { get; set; }
    public string CertificatePemBase64 { get; set; }
    public string Password { get; set; }
  }
}
