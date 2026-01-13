using System;

namespace VPortal.TenantManagement.Module.CorporateDomains
{
  public class CreateCorporateDomainRequestDto
  {
    public string DomainName { get; set; }
    public string CertificatePemBase64 { get; set; }
    public string Password { get; set; }
    public bool AcceptsClientCertificate { get; set; }
    public bool Default { get; set; }
    public bool IsSsl { get; set; }
    public int Port { get; set; }
    public Guid? AppId { get; set; }
  }
}
