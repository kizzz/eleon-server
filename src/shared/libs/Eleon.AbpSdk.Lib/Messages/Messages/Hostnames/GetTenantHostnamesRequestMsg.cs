using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.Hostnames;

[DistributedEvent]
public class GetTenantHostnamesRequestMsg
{
  public Guid? TenantId { get; set; }
}
