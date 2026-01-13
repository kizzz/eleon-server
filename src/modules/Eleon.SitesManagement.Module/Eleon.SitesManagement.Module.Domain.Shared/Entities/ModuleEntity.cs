using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.SitesManagement.Module.Entities
{
  // deprecated
  public class ModuleEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public string DisplayName { get; set; }
    public string Path { get; set; }
    public bool IsEnabled { get; set; }
    public ModuleType Type { get; set; }
    public bool IsDefault { get; set; }
    public string Source { get; set; }
    [NotMapped]
    public bool IsSystem { get; set; } = false;
    [NotMapped]
    public bool IsHidden { get; set; } = false;

    #region healthcheck
    public bool IsHealthCheckEnabled { get; set; }
    public DateTime? LastHealthCheckStatusDate { get; set; }
    public string HealthCheckStatusMessage { get; set; }
    public ServiceHealthStatus HealthCheckStatus { get; set; }
    #endregion

    public Guid? TenantId { get; set; }

    protected ModuleEntity() { }

    public ModuleEntity(Guid id)
    {
      Id = id;
    }
  }
}


