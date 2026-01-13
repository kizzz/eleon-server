using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace EventBusManagement.Module
{
  public class EventBusCheckScheduleEventService : IDistributedEventHandler<ScheduleMsg>
  {
    private readonly EventBusChecker eventBusChecker;
    private readonly EventBusConnector eventBusConnector;

    public EventBusCheckScheduleEventService(EventBusChecker eventBusChecker, EventBusConnector eventBusConnector)
    {
      this.eventBusChecker = eventBusChecker;
      this.eventBusConnector = eventBusConnector;
    }

    public async Task HandleEventAsync(ScheduleMsg eventData)
    {
      await eventBusChecker.CheckEventBuses();
      await eventBusConnector.ConnectEventBuses();
    }
  }
}
