using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Accounting.Module.Entities
{
  [Audited]
  public class AccountPackageEntity : Entity<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual DateTime NextBillingDate { get; set; }
    public virtual DateTime LastBillingDate { get; set; }//ToDay
    public virtual bool AutoSuspention { get; set; }
    public virtual bool AutoRenewal { get; set; }
    public virtual DateTime ExpiringDate { get; set; }
    public virtual AccountStatus Status { get; set; }
    public virtual Guid PackageTemplateEntityId { get; set; }
    public virtual decimal OneTimeDiscount { get; set; }
    public virtual decimal PermanentDiscount { get; set; }
    public virtual BillingPeriodType BillingPeriodType { get; set; }

    public virtual PackageTemplateEntity PackageTemplate { get; set; }

    // List of linked members
    public virtual List<MemberEntity> LinkedMembers { get; set; }
    private AccountPackageEntity() { }

    public AccountPackageEntity(Guid id) : base(id)
    {
      LinkedMembers = new List<MemberEntity>();
    }

    [NotMapped]
    public string Name { get; set; }
  }
}
