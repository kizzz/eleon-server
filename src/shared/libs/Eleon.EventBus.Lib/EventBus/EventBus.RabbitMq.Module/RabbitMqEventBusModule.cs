using Common.EventBus.Abstractions.Module;
using EventBus.RabbitMq.Module;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.RabbitMQ;

namespace EventBus.Nats
{
  [DependsOn(typeof(CommonEventBusAbstractionsModule), typeof(AbpRabbitMqModule))]
  public class RabbitMqEventBusModule : AbpModule
  {
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
      context.Services.AddAbpDynamicOptions<AbpRabbitMqOptions, RabbitMqDynamicOptionsManager>();
    }
  }
}
