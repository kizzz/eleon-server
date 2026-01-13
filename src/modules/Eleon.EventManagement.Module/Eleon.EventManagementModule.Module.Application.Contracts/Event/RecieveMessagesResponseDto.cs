using EventManagementModule.Module.Application.Contracts.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Event;
public class RecieveMessagesResponseDto
{
  public string QueueStatus { get; set; }
  public int MessagesLeft { get; set; }
  public List<FullEventDto> Messages { get; set; }
}
