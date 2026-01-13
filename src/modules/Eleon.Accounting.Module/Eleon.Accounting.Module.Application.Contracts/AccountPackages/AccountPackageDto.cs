using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.Accounting.Module.Accounts;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounting.Module.AccountPackages
{
  public class AccountPackageDto
  {
    public Guid Id { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime LastBillingDate { get; set; }
    public bool AutoSuspention { get; set; }
    public bool AutoRenewal { get; set; }
    public DateTime ExpiringDate { get; set; }
    public AccountStatus Status { get; set; }
    public string Name { get; set; }
    public Guid PackageTemplateEntityId { get; set; }
    public BillingPeriodType BillingPeriodType { get; set; }
    public decimal OneTimeDiscount { get; set; }
    public decimal PermanentDiscount { get; set; }
    public List<MemberDto> LinkedMembers { get; set; }
  }
}
