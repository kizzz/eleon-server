using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace EventBusManagement.Module.EntityFrameworkCore
{
  public class EventBusEntity : AggregateRoot<Guid>, IMultiTenant
  {
    public EventBusEntity(Guid id, EventBusProvider provider, string providerOptions, EventBusStatus status)
    {
      Id = id;
      Provider = provider;
      ProviderOptions = providerOptions;
      Status = status;
    }

    protected EventBusEntity() { }

    public EventBusProvider Provider { get; set; }
    public string ProviderOptions { get; set; }
    public EventBusStatus Status { get; set; }
    public bool IsDefault { get; set; }
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
  }
}
