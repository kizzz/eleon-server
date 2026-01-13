using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementModule.Module.Application.Contracts.Queue;
public class UpdateQueueRequestDto
{
  //public Guid? TenantId { get; set; }
  public string Name { get; set; }

  public string NewName { get; set; }
  public string DisplayName { get; set; }
  public string Forwarding { get; set; }
  public int MessagesLimit { get; set; }
}
