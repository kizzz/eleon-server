using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementModule.Module.Application.Contracts.QueueDefenition;
public class CreateQueueDefinitionRequestDto
{
  public string Name { get; set; }
  public string Messages { get; set; }
  //public string Type { get; set; }
  public int MessagesLimit { get; set; }
}
