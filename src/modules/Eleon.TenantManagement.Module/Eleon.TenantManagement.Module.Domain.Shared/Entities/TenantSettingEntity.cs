using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.TenantManagement.Module.Entities
{
  public class TenantSettingEntity : FullAuditedAggregateRoot<Guid>
  {
    public TenantSettingEntity(Guid id, Guid? tenantId)
    {
      Id = id;
      TenantId = tenantId;
      Status = TenantStatus.Active;
    }

    protected TenantSettingEntity() { }

    public Guid? TenantId { get; set; }

    public bool TenantIsolationEnabled { get; set; }

    public string TenantCertificateHash { get; set; } = string.Empty;

    public bool IpIsolationEnabled { get; set; }

    public TenantStatus Status { get; set; }

    public virtual List<TenantHostnameEntity> Hostnames { get; set; } = new();
    public virtual List<TenantExternalLoginProviderEntity> ExternalProviders { get; set; } = new();
    public virtual List<TenantWhitelistedIpEntity> WhitelistedIps { get; set; } = new();
    public virtual List<TenantContentSecurityHostEntity> ContentSecurityHosts { get; set; } = new();
    public virtual TenantAppearanceSettingEntity? AppearanceSettings { get; set; }
  }
}
