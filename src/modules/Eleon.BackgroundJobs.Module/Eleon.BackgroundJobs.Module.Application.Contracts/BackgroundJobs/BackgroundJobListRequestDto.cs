using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobListRequestDto : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
    public IList<BackgroundJobStatus> StatusFilter { get; set; }
    public IList<string> ObjectTypeFilter { get; set; }
    public IList<string> TypeFilter { get; set; }
    public DateTime? CreationDateFilterStart { get; set; }
    public DateTime? CreationDateFilterEnd { get; set; }
    public DateTime? LastExecutionDateFilterStart { get; set; }
    public DateTime? LastExecutionDateFilterEnd { get; set; }
  }
}
