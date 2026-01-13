using Common.EventBus.Module;
using Common.EventBus.Module.Options;
using Common.Module.Constants;
using EventBusManagement.Module.EntityFrameworkCore;
using Logging.Module;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EventBusManagement.Module
{
  public class EventBusConnector : ISingletonDependency
  {
    private readonly IVportalLogger<EventBusConnector> logger;
    private readonly IEventBusRepository eventBusRepository;
    private readonly DistributedBusResolver distributedBusResolver;
    private readonly IOptions<EventBusManagementOptions> options;
    private DateTime LastCheckUtc = DateTime.MinValue;
    private bool CheckRunning = false;
    private object CheckLocker = new object();

    public EventBusConnector(
        IVportalLogger<EventBusConnector> logger,
        IEventBusRepository eventBusRepository,
        DistributedBusResolver distributedBusResolver,
        IOptions<EventBusManagementOptions> options)
    {
      this.logger = logger;
      this.eventBusRepository = eventBusRepository;
      this.distributedBusResolver = distributedBusResolver;
      this.options = options;
    }

    public async Task ConnectEventBuses()
    {
      try
      {
        lock (CheckLocker)
        {
          bool isTimeToConnect = DateTime.UtcNow.Subtract(LastCheckUtc) < TimeSpan.FromMinutes(options.Value.BusConnectionInterval);
          if (CheckRunning || !isTimeToConnect)
          {
            return;
          }

          CheckRunning = true;
        }

        var buses = await eventBusRepository.GetListAsync();

        foreach (var bus in buses)
        {
          var options = new EventBusOptions()
          {
            Provider = bus.Provider,
            ProviderOptionsJson = bus.ProviderOptions,
          };

          await distributedBusResolver.ConnectEventBus(bus.TenantId, options, bus.Id);
        }

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

    public async Task ConnectEventBus(Guid busId)
    {
      try
      {
        var bus = await eventBusRepository.GetAsync(busId);

        var options = new EventBusOptions()
        {
          Provider = bus.Provider,
          ProviderOptionsJson = bus.ProviderOptions,
        };

        await distributedBusResolver.ConnectEventBus(bus.TenantId, options, bus.Id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
