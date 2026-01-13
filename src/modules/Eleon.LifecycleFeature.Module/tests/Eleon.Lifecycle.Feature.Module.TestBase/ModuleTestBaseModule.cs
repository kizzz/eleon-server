using Eleon.Logging.Lib.VportalLogging;
using EleonsoftSdk.modules.Helpers.Module;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace VPortal.Lifecycle.Feature.Module;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpGuidsModule)
)]
public class ModuleTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysAllowAuthorization();
        context.Services.AddLogging();
        context.Services.AddVportalLogging();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // Initialize StaticServicesAccessor for services that use it
        StaticServicesAccessor.Initialize(context.ServiceProvider);
        
        // Skip seeding if Identity tables don't exist (they'll be created by ModuleDomainTestModule)
        try
        {
            SeedTestData(context);
        }
        catch
        {
            // Ignore seeding errors - tables might not be created yet
        }
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}
