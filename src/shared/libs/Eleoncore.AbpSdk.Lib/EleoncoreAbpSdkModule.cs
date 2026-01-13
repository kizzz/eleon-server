using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Migration.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Module;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Eleoncore.Abp.Sdk
{
  [DependsOn(
      typeof(AbpEntityFrameworkCoreModule)
  )]
  public class EleoncoreAbpSdkModule : AbpModule
  {
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
      Configure<AbpDbContextOptions>(options =>
      {
        options.UseSqlServer(
                  opt => opt.UseCompatibilityLevel(context.Services.GetConfiguration().GetValue("SqlServer:CompatibilityLevel", 120)));
      });

      context.Services.AddSingleton<IDbMigrationService, DefaultEleoncoreDbMigrationService>();
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
      StaticServicesAccessor.Initialize(context.ServiceProvider);

    }
  }
}

