using Volo.Abp.Application.Dtos;

namespace VPortal.JobScheduler.Module.Tasks
{
  public class TaskListRequestDto : PagedAndSortedResultRequestDto
  {
    public string NameFilter { get; set; }
  }
}
