using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.AppConfig;

[DistributedEvent]
public class GetBaseAppConfigRequestMsg
{
  public Guid? UserId { get; set; }
}
