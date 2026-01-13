using System;
using Volo.Abp.Application.Dtos;

namespace VPortal.Accounting.Module.AccountPackages
{
  public class AccountPackageListRequestDto : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
    public DateTime? DateFilterStart { get; set; }
    public DateTime? DateFilterEnd { get; set; }
    public DateTime? NextBillingDateFilterStart { get; set; }
    public DateTime? NextBillingDateFilterEnd { get; set; }
    public DateTime? ExpiringDateFilterStart { get; set; }
    public DateTime? ExpiringDateFilterEnd { get; set; }
  }
}
