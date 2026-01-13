using Common.EventBus.Abstractions.Module;
using Common.EventBus.Module;
using Common.Module.Constants;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Eleonsoft.Host;

namespace Eleonsoft.Host.Test.TestBase;

/// <summary>
/// Test startup module for cross-module integration tests
/// Configures in-memory event bus and SQLite database for testing
/// </summary>
[DependsOn(
  typeof(MaximalHostModule),
  typeof(AbpEntityFrameworkCoreSqliteModule),
  typeof(AbpTestBaseModule)
)]
public class CrossModuleTestStartupModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    // Configure in-memory event bus for cross-module tests
    // This must be done in PreConfigureServices before CommonEventBusModule configures it
    var existingConfig = context.Services.GetConfiguration();
    var configurationBuilder = new ConfigurationBuilder();

    if (existingConfig != null)
    {
      configurationBuilder.AddConfiguration(existingConfig);
    }

    configurationBuilder.AddInMemoryCollection(
      new Dictionary<string, string?>
      {
        { "EventBus:Provider", EventBusProvider.InMemory.ToString() },
      }
    );

    var configuration = configurationBuilder.Build();
    context.Services.AddSingleton<IConfiguration>(configuration);
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    // Disable UOW transactions for testing
    context.Services.AddAlwaysDisableUnitOfWorkTransaction();

    // Ensure CurrentEvent is available for SystemDistributedEventBus in test container
    context.Services.AddTransient<CurrentEvent>();

    // Override event bus configuration to use in-memory
    // This will override any configuration from CommonEventBusModule
    Configure<EventBusOptions>(options =>
    {
      options.Provider = EventBusProvider.InMemory;
      options.ProviderOptionsJson = "{}";
    });

    // Configure SQLite in-memory database
    var sqliteConnection = CreateDatabaseAndGetConnection();

    context.Services.Configure<AbpDbContextOptions>(options =>
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

    // Note: We can't create all DbContexts here as they're module-specific
    // Each module's test startup module will handle its own DbContext creation
    // This connection will be used by the first module that needs it

    return connection;
  }
}
