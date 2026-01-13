using EleonS3.Application.EventHandlers;
using EleonS3.Domain;
using EleonsoftSdk.modules.StorageProvider.Module;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using VPortal.MassTransit.RabbitMQ;

namespace VPortal.FileManager.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(ModuleDomainSharedModule))]
public class ModuleDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ModuleDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ModuleDomainModule>(validate: true);
    });
    context.Services.AddSingleton<VfsStorageProviderCacheManager>();
    context.Services.AddTransient<StorageProviderChangedEventHandler>();
    context.Services.AddTransient<TelemetryProviderIdChangedEventHandler>();
  }

  private void ConfigureConsumers(MassTransitRabbitMqOptions o)
  {
  }
}
