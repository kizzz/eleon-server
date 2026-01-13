using Common.EventBus.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace EventBusManagement.Module
{
  public class CheckEventBusEventService : ITransientDependency, IDistributedEventHandler<CheckEventBusEventService>
  {
    private readonly IResponseContext responseContext;

    public CheckEventBusEventService(IResponseContext responseContext)
    {
      this.responseContext = responseContext;
    }

    public async Task HandleEventAsync(CheckEventBusEventService eventData)
    {
      await responseContext.RespondAsync(eventData);
    }
  }
}
