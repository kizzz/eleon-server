using Common.Module.Events;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System;

namespace EleonsoftAbp.Messages.AppConfig;

[DistributedEvent]
public class EnrichApplicationsConfigurationRequestMsg
{
  public Guid? TenantId { get; set; }
}
