using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.Messages.User;

[DistributedEvent]
public class GetUserByIdRequestMsg
{
  public Guid UserId { get; set; }
}
