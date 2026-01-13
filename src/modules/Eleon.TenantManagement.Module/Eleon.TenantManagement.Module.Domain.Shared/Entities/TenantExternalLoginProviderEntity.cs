using Common.Module.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.TenantManagement.Module.Entities
{
  public class TenantExternalLoginProviderEntity : FullAuditedEntity<Guid>
  {
    public TenantExternalLoginProviderEntity(Guid id)
    {
      Id = id;
    }

    protected TenantExternalLoginProviderEntity() { }

    public ExternalLoginProviderType Type { get; set; }

    public bool Enabled { get; set; }

    public string Data { get; set; }

    public string AdminIdentifier { get; set; }

    [NotMapped]
    public string Authority { get; set; }

    [NotMapped]
    public string ClientId { get; set; }

    [NotMapped]
    public string ClientSecret { get; set; }
  }
}
