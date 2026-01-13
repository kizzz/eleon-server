using EventBusManagement.Module.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.GatewayManagement.Module.EntityFrameworkCore;

[DependsOn(
    typeof(GatewayManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class GatewayManagementEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<GatewayManagementDbContext>(options =>
    {
      options.AddRepository<EventBusEntity, EventBusRepository>();
    });
  }
}
