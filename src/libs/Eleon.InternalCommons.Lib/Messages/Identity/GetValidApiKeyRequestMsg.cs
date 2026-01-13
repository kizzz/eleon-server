using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Identity;

[Common.Module.Events.DistributedEvent]
public class GetValidApiKeyRequestMsg
{
  public string ApiKey { get; set; }
}
