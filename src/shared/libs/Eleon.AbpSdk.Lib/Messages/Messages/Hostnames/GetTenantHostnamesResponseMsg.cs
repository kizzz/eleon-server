using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.Hostnames;
[DistributedEvent]
public class GetTenantHostnamesResponseMsg
{
  public List<HostnameEto> Hostnames { get; set; } = new List<HostnameEto>();
}
