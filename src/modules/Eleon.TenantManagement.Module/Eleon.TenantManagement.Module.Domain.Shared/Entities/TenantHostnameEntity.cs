using Common.Module.Constants;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Consts;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.TenantManagement.Module.Entities
{
  public class TenantHostnameEntity : FullAuditedEntity<Guid>
  {
    protected TenantHostnameEntity() { }

    public TenantHostnameEntity(Guid id)
    {
      Id = id;
    }



    public Guid? TenantId { get; set; }
    public string Domain { get; set; }
    public string Subdomain { get; set; }
    public int Port { get; set; }
    public bool IsSsl { get; set; }
    public VportalApplicationType ApplicationType { get; set; }
    public bool Internal { get; set; }
    public bool AcceptsClientCertificate { get; set; }
    public bool Default { get; set; }

    public Guid? AppId { get; set; }
    public HostnameStatus Status { get; set; } = HostnameStatus.Active;


    [NotMapped]
    public string Url => (IsSsl ? "https://" : "http://") + HostnameWithPort;

    [NotMapped]
    public string Hostname => (Subdomain.IsNullOrWhiteSpace() ? string.Empty : Subdomain + ".") + Domain;

    [NotMapped]
    public string HostnameWithPort => Hostname + (Port == 80 || Port == 443 ? string.Empty : ":" + Port.ToString());
  }
}
