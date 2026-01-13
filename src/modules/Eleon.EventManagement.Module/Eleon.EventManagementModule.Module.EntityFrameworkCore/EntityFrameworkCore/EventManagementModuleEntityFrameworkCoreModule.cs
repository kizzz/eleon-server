using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using VPortal.EventManagementModule.Module.EntityFrameworkCor.Repositories;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore;

[DependsOn(
    typeof(EventManagementModuleDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class EventManagementModuleEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<EventManagementModuleDbContext>(options =>
    {
      options.AddDefaultRepositories();
      options.AddRepository<QueueDefinitionEntity, QueueDefinitionRepository>();
      options.AddRepository<QueueEntity, QueueRepository>();
    });
  }
}
