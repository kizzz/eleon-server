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
  public class AccountEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual string DataSourceUid { get; set; }
    public virtual string DataSourceName { get; set; }
    public virtual string CompanyUid { get; set; }
    public virtual string CompanyName { get; set; }
    public virtual Guid? OrganizationUnitId { get; set; }
    public virtual string OrganizationUnitName { get; set; }
    public virtual decimal CurrentBalance { get; set; }//ne trogat
    // Navigation property to Invoices
    public virtual List<InvoiceEntity> Invoices { get; set; }
    // Name of the account
    public virtual string AccountName { get; set; }
    // Current status of the account
    public virtual AccountStatus AccountStatus { get; set; }
    // Package associated with the account
    public virtual List<AccountPackageEntity> AccountPackages { get; set; }
    public virtual BillingInformationEntity BillingInformation { get; set; }

    // New properties
    public virtual Guid OwnerId { get; set; }
    public virtual List<MemberEntity> Members { get; set; }


    private AccountEntity() { }

    public AccountEntity(Guid id) : base(id)
    {
      Invoices = new List<InvoiceEntity>();
      AccountPackages = new List<AccountPackageEntity>();
      Members = new List<MemberEntity>();
    }

    #region Display Properties
    // ❌ DELETED:
    // [NotMapped] public DocumentVersionEntity DocumentVersionEntity { get; set; }
    #endregion
  }
}
