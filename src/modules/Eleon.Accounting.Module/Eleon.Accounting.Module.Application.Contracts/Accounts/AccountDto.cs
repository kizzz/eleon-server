using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.Accounting.Module.AccountPackages;
using VPortal.Accounting.Module.BillingInformations;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Invoices;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Accounting.Module.Accounts
{
  public class AccountDto : AccountHeaderDto
  {
    public BillingInformationDto BillingInformation { get; set; }
    public List<MemberDto> Members { get; set; }
    public List<AccountPackageDto> AccountPackages { get; set; }
    public List<InvoiceDto> Invoices { get; set; }
  }
}
