using Common.EventBus.Abstractions.Module.Options;
using Common.EventBus.Module.Options;
using Common.Module.Constants;
using EventBus.MassTransit.Module;
using EventBusManagement.Module.EntityFrameworkCore;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.MassTransit.RabbitMQ;

namespace EventBusManagement.Module
{

  public class EventBusManager : DomainService, ITransientDependency
  {
    private readonly IVportalLogger<EventBusManager> logger;
    private readonly IConfiguration configuration;
    private readonly IEventBusRepository repository;

    public EventBusManager(
        IVportalLogger<EventBusManager> logger,
        IConfiguration configuration,
        IEventBusRepository repository)
    {
      this.logger = logger;
      this.configuration = configuration;
      this.repository = repository;
    }

    public async Task<EventBusEntity> GetEventBus(Guid id)
    {
      EventBusEntity result = null;
      try
      {
        result = await repository.GetAsync(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<EventBusEntity> GetDefaultEventBus()
    {
      EventBusEntity result = null;
      try
      {
        var buses = await repository.GetListAsync();
        result = buses.FirstOrDefault(x => x.IsDefault);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task AddEventBus(EventBusEntity bus)
    {
      try
      {
        await repository.InsertAsync(bus);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task UpdateEventBus(EventBusEntity bus)
    {
      try
      {
        var entity = await repository.GetAsync(bus.Id);
        entity.Provider = bus.Provider;
        entity.ProviderOptions = bus.ProviderOptions;
        entity.IsDefault = bus.IsDefault;
        entity.Status = bus.Status;
        entity.Name = bus.Name;

        await repository.UpdateAsync(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<EventBusEntity>> GetEventBusList()
    {
      List<EventBusEntity> result = null;
      try
      {
        result = await repository.GetListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<Dictionary<EventBusProvider, string>> GetSettingTemplates()
    {
      var result = new Dictionary<EventBusProvider, string>();
      try
      {
        result[EventBusProvider.NATS] = JsonConvert.SerializeObject(new NatsOptions()
        {
          Port = 4222,
          Url = "nats://localhost",
        });

        result[EventBusProvider.RabbitMQ] = JsonConvert.SerializeObject(new RabbitMqOptions()
        {

        });

        result[EventBusProvider.InMemory] = JsonConvert.SerializeObject(new InMemoryOptions()
        {

        });

        result[EventBusProvider.MassTransit] = JsonConvert.SerializeObject(new MassTransitOptions()
        {
          Type = MassTransitOptions.TransportType.RabbitMQ,
          RabbitMqOptions = new MassTransitRabbitMqOptions()
          {
            Host = "127.0.0.1",
            Port = 5672,
            VirtualHost = "/",
            Username = "guest",
            Password = "guest",
            UseSsl = false,
            UseCluster = false,
            ClusterNodes = ["127.0.0.1"],
            DefaultQueuePrefix = configuration["ApplicationName"] ?? "Common",
            DefaultConcurrentMessageLimit = 1,
            DefaultPrefetchCount = 4,
            DefaultDurable = true,
            DefaultAutoDelete = false,
            DefaultExchangeType = "fanout",
          },
        });
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
