using Castle.DynamicProxy;
using Common.EventBus.Module;
using Common.Module.Helpers;
using EventBus.Nats;
using Logging.Module;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.Threading;

namespace EventBus.MassTransit.Module
{
  public class MassTransitHostStore
  {
    private static ConsumerProxyGenerator ProxyGenerator = new();
    private List<Action<IBusRegistrationConfigurator>> busConfigurations = new();
    private List<Action<IServiceCollection>> postConfigurations = new();
    private IHostBuilder _hostBuilder;
    private readonly IMassTransitTransport _transport;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _hostServiceProvider;

    private MassTransitHostStore(IServiceScopeFactory scopeFactory)
    {
      _hostServiceProvider = scopeFactory.CreateScope().ServiceProvider;
      _hostBuilder = new HostBuilder()
           .UseServiceProviderFactory(context => new DefaultServiceProviderFactory());
        
      _configuration = _hostServiceProvider.GetRequiredService<IConfiguration>();

      var options = _hostServiceProvider.GetRequiredService<IOptions<EventBusOptions>>().Value;
      var providerOptions = JsonConvert.DeserializeObject<MassTransitOptions>(options.ProviderOptionsJson);
      var transportResolver = _hostServiceProvider.GetRequiredService<MassTransitTransportResolver>();
      _transport = transportResolver.Resolve(providerOptions);
    }

    public MassTransitHost Build()
    {
      MassTransitDistributedEventBus.Initialize(ActivatorUtilities.CreateInstance<MassTransitDistributedEventBus>(_hostServiceProvider));
      _hostBuilder.ConfigureServices(ConfigureServices);
      var host = new MassTransitHost(_hostServiceProvider.GetRequiredService<ILogger<MassTransitHost>>(), _hostBuilder.Build());
      host.Start();
      return host;
    }

    public void AddConsumer(Type eventType, Func<MassTransitConsumeContext, Task> consumer)
    {
      var proxyOptions = new ProxyGenerationOptions();

      string applicationName = _configuration["ApplicationName"] ?? "Common";
      var typeName = $"{applicationName}_{TypeSanitizeHelper.SanitizeEventTypeName(eventType.Name)}";


      var consumerProxyType = ProxyGenerator.CreateConsumerType(eventType, proxyOptions);

      busConfigurations.Add(cfg =>
      {
        cfg.AddConsumer(consumerProxyType)
                  .Endpoint(cfg =>
                  {
                cfg.Name = typeName;
              });
      });

      postConfigurations.Add(services =>
      {
        services.AddScoped(consumerProxyType, provider =>
              {
                var consumerInterceptor = new MassTransitConsumerProxy(
                      eventType,
                      consumer,
                      _hostServiceProvider.GetRequiredService<EventContextManager>());
            return ProxyGenerator.ActivateConsumerType(consumerProxyType, proxyOptions, consumerInterceptor);
          });
      });
    }

    private void ConfigureServices(IServiceCollection services)
    {
      string applicationName = _configuration["ApplicationName"] ?? "Common";
      services.AddMassTransit(x =>
      {
        x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(applicationName));
        foreach (var cfg in busConfigurations)
        {
          cfg(x);
        }

        _transport.ConfigureBus(x);
      });

      foreach (var cfg in postConfigurations)
      {
        cfg(services);
      }
    }


    private static MassTransitHost? _host = null;
    private static SemaphoreSlim hostAccessor = new SemaphoreSlim(1, 1);
    public static bool IsConnected => _host != null;

    public static MassTransitHost GetHost()
    {
      if (_host != null)
      {
        return _host;
      }

      using var l = hostAccessor.Lock();

      if (_host != null)
      {
        return _host;
      }

      if (_builder == null)
      {
        throw new Exception("MassTransit bus is not initialized.");
      }

      _host = _builder.Build();
      return _host;
    }

    private static MassTransitHostStore? _builder = null;

    public static MassTransitHostStore GetOrInitialize(IServiceScopeFactory scopeFactory)
    {
      if (_builder != null)
      {
        return _builder;
      }

      _builder = new MassTransitHostStore(scopeFactory);

      return _builder;
    }
  }
}
