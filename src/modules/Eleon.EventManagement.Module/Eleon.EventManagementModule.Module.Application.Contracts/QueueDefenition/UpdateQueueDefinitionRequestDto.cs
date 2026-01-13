using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementModule.Module.Application.Contracts.QueueDefenition;
public class UpdateQueueDefinitionRequestDto
{
  public Guid Id { get; set; }
  public int MessagesLimit { get; set; }
}
