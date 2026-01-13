using Common.Module.Constants;
using System;
using VPortal.Accounting.Module.AccountPackages;
using VPortal.Accounting.Module.BillingInformations;
using VPortal.Accounting.Module.Invoices;

namespace VPortal.Accounting.Module.Accounts
{
  public class AccountHeaderDto
  {
    public Guid Id { get; set; }
    public string DataSourceUid { get; set; }
    public string DataSourceName { get; set; }
    public string CompanyUid { get; set; }
    public string CompanyName { get; set; }
    public Guid? OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; }
    public decimal CurrentBalance { get; set; }
    public string AccountName { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public Guid? CreatorId { get; set; }
    // public DocumentVersionEntity DocumentVersionEntity { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid OwnerId { get; set; }
    public Guid? TenantId { get; set; }

  }
}
