using Volo.Abp.Application.Dtos;

namespace VPortal.Collaboration.Feature.Module.ChatRooms
{
  public class ChatListRequestDto : PagedAndSortedResultRequestDto
  {
    public string NameFilter { get; set; }
  }
}
