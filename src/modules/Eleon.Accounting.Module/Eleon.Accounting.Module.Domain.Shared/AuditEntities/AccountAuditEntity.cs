using Common.Module.Constants;
using System;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Accounting.Module.AuditEntities
{
  public class AccountAuditEntity
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string DataSourceUid { get; set; }
    public string DataSourceName { get; set; }
    public string CompanyUid { get; set; }
    public string CompanyName { get; set; }
    public Guid? OrganizationUnitId { get; set; }
    public Guid? OwnerId { get; set; }
    public string OrganizationUnitName { get; set; }
    public decimal CurrentBalance { get; set; }//ne trogat
    public string SystemCurrency { get; set; }
    public string AccountName { get; set; }
    // Current status of the account
    public AccountStatus AccountStatus { get; set; }

    // Package associated with the account
    public Guid? BillingInformationId { get; set; }

    //public DocumentVersionEntity DocumentVersionEntity { get; set; }
    public DateTime CreationTime { get; set; }
  }
}
