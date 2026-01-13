using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.RabbitMQ;

namespace EventBus.RabbitMq.Module
{
  [Volo.Abp.DependencyInjection.Dependency(ServiceLifetime.Singleton, ReplaceServices = true)]
  [ExposeServices(typeof(ConnectionPool), typeof(IConnectionPool))]
  public class RabbitMqDynamicConnectionPool : ConnectionPool
  {
    public RabbitMqDynamicConnectionPool(IOptions<AbpRabbitMqOptions> options) : base(options)
    {
      options.SetAsync().GetAwaiter().GetResult();
    }
  }
}
