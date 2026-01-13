using System;

namespace VPortal.TenantManagement.Module.CorporateDomains
{
  public class RemoveCorporateDomainRequestDto
  {
    public Guid? TenantId { get; set; }
    public Guid HostnameId { get; set; }
  }
}
