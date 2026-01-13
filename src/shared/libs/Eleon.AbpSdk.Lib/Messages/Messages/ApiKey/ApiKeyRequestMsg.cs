using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.ApiKey;

[Common.Module.Events.DistributedEvent]
public class ApiKeyRequestMsg
{
  public string KeyId { get; set; }
}
