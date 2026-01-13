

using Volo.Abp.Application.Dtos;

namespace EventManagementModule.Module.Application.Contracts.Event;
public class MessagesPagedAndSortedResultRequestDto : PagedAndSortedResultRequestDto
{
  public Guid QueueId { get; set; }
}
