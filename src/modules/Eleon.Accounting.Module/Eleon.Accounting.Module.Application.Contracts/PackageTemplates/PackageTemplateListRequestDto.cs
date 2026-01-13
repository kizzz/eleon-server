using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.Accounting.Module.PackageTemplates
{
  public class PackageTemplateListRequestDto : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
    public DateTime? DateFilterStart { get; set; }
    public DateTime? DateFilterEnd { get; set; }
    public IList<BillingPeriodType> BillingPeriodTypeFilter { get; set; }
  }
}
