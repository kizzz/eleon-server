using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

namespace CollaborationModule.Test.TestBase;

/// <summary>
/// Test startup module for Collaboration module integration tests.
/// Configures SQLite in-memory database for testing.
/// </summary>
[DependsOn(
    typeof(CollaborationEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
)]
public class CollaborationTestStartupModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

        var sqliteConnection = CreateDatabaseAndGetConnection();

        // Register the connection in DI so it can be disposed properly
        context.Services.AddSingleton(sqliteConnection);

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(configurationContext =>
            {
                configurationContext.UseSqlite(sqliteConnection);
            });
        });

        Configure<AbpUnitOfWorkOptions>(options =>
        {
            options.IsTransactional = false; // Disable transactions for tests
        });
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        var connection = context.ServiceProvider.GetService<SqliteConnection>();
        connection?.Dispose();
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // Create database schema
        new CollaborationDbContext(
            new DbContextOptionsBuilder<CollaborationDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }
}
