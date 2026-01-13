using Microsoft.Extensions.Options;
using Volo.Abp.Options;
using Volo.Abp.RabbitMQ;

namespace EventBus.RabbitMq.Module
{
  public class RabbitMqDynamicOptionsManager : AbpDynamicOptionsManager<AbpRabbitMqOptions>
  {
    private readonly RabbitMqDynamicConnections optionsStore;

    public RabbitMqDynamicOptionsManager(
        IOptionsFactory<AbpRabbitMqOptions> factory,
        RabbitMqDynamicConnections optionsStore)
        : base(factory)
    {
      this.optionsStore = optionsStore;
    }

    protected override Task OverrideOptionsAsync(string name, AbpRabbitMqOptions options)
    {
      foreach (var key in optionsStore.Keys)
      {
        options.Connections[key] = optionsStore[key];
      }

      return Task.CompletedTask;
    }
  }
}
