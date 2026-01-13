using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.ChatRooms;
public class UpdateChatRequestDto
{
  public Guid ChatId { get; set; }
  public string ChatName { get; set; }
  public List<string> Tags { get; set; }
  public bool IsPublic { get; set; }
  public ChatMemberRole DefaultRole { get; set; }
}
