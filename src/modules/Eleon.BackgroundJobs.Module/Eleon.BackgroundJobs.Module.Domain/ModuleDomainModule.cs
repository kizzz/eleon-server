using BackgroundJobs.Module.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using VPortal.MassTransit.RabbitMQ;

namespace VPortal.BackgroundJobs.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ModuleDomainSharedModule)
)]
public class ModuleDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    base.ConfigureServices(context);
    context.Services.AddAutoMapperObjectMapper<ModuleDomainModule>();

    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddProfile<BackgroundJobsModuleDomainAutoMapperProfile>(validate: true);
    });

    // Explicitly register the interface for BackgroundJobDomainService
    // ABP auto-registers domain services, but we need to ensure the interface is also registered
    context.Services.AddTransient<DomainServices.IBackgroundJobDomainService, DomainServices.BackgroundJobDomainService>();
  }
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {

  }


  public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
  {
  }

  public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
  {
    var configuration = context.GetConfiguration();

    // Register the ScheduleBackgroundWorker if BackgroundJobs is enabled
    if (configuration.GetValue<bool>("BackgroundJobs") != false)
    {
      // IMPORTANT: We need to use OnPostApplicationInitializationAsync instead of OnApplicationInitializationAsync
      // otherwise the IDistributedEventBus could not be ready when the worker starts
      // and it could cause an infinite delay when trying to get the service
      await context.AddBackgroundWorkerAsync<ScheduleBackgroundWorker>();
    }
  }
}
