using Common.Module.Events;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.AppConfig;

[DistributedEvent]
public class GetBaseAppConfigResponseMsg
{
  public EleoncoreApplicationConfigurationValueObject ApplicationConfiguration { get; set; }
  public bool Success { get; set; }
}
