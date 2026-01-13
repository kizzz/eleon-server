using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.Lifecycle.Feature.Module.Audits
{
  public class PendingApprovalRequestDto : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
    public DateTime? StatusDateFilterStart { get; set; }
    public DateTime? StatusDateFilterEnd { get; set; }
    public IList<string>? ObjectTypeFilter { get; set; }
    public Guid? UserId { get; set; }
    public Guid? StatesGroupTemplateId { get; set; }
  }
}
