using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.AutoMapper;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using VPortal.Lifecycle.Feature.Module;
using VPortal.Lifecycle.Feature.Module.EntityFrameworkCore;

namespace VPortal.Lifecycle.Feature.Module;

[DependsOn(
    typeof(ModuleApplicationModule),
    typeof(ModuleDomainTestModule),
    typeof(LifecycleFeatureEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
public class ModuleApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
        context.Services.AddLogging();
        context.Services.AddVportalLogging();
        
        // Configure AutoMapper for tests - ensure profiles are loaded
        context.Services.AddAutoMapperObjectMapper<ModuleApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ModuleApplicationModule>(validate: false);
        });

        var sqliteConnection = CreateDatabaseAndGetConnection();

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(configurationContext =>
            {
                configurationContext.UseSqlite(sqliteConnection);
            });
        });
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        new LifecycleFeatureDbContext(
            new DbContextOptionsBuilder<LifecycleFeatureDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }
}
