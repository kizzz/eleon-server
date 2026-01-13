using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Accounting.Module.Entities
{
  [Audited]
  public class PackageTemplateEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual string PackageName { get; set; }
    public virtual PackageType PackageType { get; set; }
    public virtual string Description { get; set; }
    public virtual int MaxMembers { get; set; }
    public virtual decimal Price { get; set; }
    public virtual BillingPeriodType BillingPeriodType { get; set; }

    // List of Modules enabled in this package by default
    public virtual List<PackageTemplateModuleEntity> PackageTemplateModules { get; set; }

    private PackageTemplateEntity() { }

    public PackageTemplateEntity(Guid id) : base(id)
    {
      PackageTemplateModules = new List<PackageTemplateModuleEntity>();
    }

    #region Display Properties
    [NotMapped]
    public string SystemCurrency { get; set; }
    #endregion
  }
}
