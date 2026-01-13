using Common.EventBus.Module;
using EventBusManagement.Module.EntityFrameworkCore;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace EventBusManagement.Module
{
  public class EventBusChecker : ISingletonDependency
  {
    private readonly IVportalLogger<EventBusChecker> logger;
    private readonly IEventBusRepository eventBusRepository;
    private readonly DistributedBusResolver distributedBusResolver;
    private readonly IOptions<EventBusManagementOptions> options;
    private DateTime LastCheckUtc = DateTime.UtcNow;
    private bool CheckRunning = false;
    private object CheckLocker = new object();

    public EventBusChecker(
        IVportalLogger<EventBusChecker> logger,
        IEventBusRepository eventBusRepository,
        DistributedBusResolver distributedBusResolver,
        IOptions<EventBusManagementOptions> options)
    {
      this.logger = logger;
      this.eventBusRepository = eventBusRepository;
      this.distributedBusResolver = distributedBusResolver;
      this.options = options;
    }

    public async Task CheckEventBuses()
    {
      try
      {
        lock (CheckLocker)
        {
          bool isTimeToConnect = DateTime.UtcNow.Subtract(LastCheckUtc) < TimeSpan.FromMinutes(options.Value.BusCheckInterval);
          if (CheckRunning || !isTimeToConnect)
          {
            return;
          }

          CheckRunning = true;
        }

        var busEntities = await eventBusRepository.GetListAsync();
        var connectedBuses = distributedBusResolver.GetConnectedEventBuses();

        var failedBuses = await DetectFailedEventBuses(connectedBuses);

        lock (CheckLocker)
        {
          LastCheckUtc = DateTime.UtcNow;
          CheckRunning = true;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task<List<IDistributedEventBus>> DetectFailedEventBuses(List<IDistributedEventBus> buses)
    {
      var failedBuses = new List<IDistributedEventBus>();
      foreach (var bus in buses)
      {
        bool responseReceived;
        try
        {
          await bus.RequestAsync<EventBusCheckedMsg>(new CheckEventBusMsg(), timeoutSeconds: 10);
          responseReceived = true;
        }
        catch (Exception)
        {
          responseReceived = false;
        }

        if (!responseReceived)
        {
          failedBuses.Add(bus);
        }
      }

      return failedBuses;
    }
  }
}
