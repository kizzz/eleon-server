using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.Collaboration.Feature.Module.ChatInteractions
{
  public class LastChatsRequestDto
  {
    public int Skip { get; set; }
    public int Take { get; set; }
    public string NameFilter { get; set; }
    public List<ChatRoomType> ChatRoomTypes { get; set; }
    public bool IsArchived { get; set; }
    public bool IsChannel { get; set; }
    public List<string> Tags { get; set; }
  }
}
