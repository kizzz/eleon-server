using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.ApiKey;

[Common.Module.Events.DistributedEvent]
public class ApiKeyDeletedMsg
{
  public bool IsSuccessfully { get; set; }
  public string ApiKeyId { get; set; }
}
