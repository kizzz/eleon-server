using System;
using System.Collections.Generic;
using Common.EventBus.Abstractions.Module;
using Common.EventBus.Module;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Eleonsoft.Host;

namespace Eleonsoft.Host.Test.TestBase;

/// <summary>
/// Test startup module for cross-module integration tests against a real database.
/// Uses in-memory event bus and SQL Server when a connection string is provided.
/// Falls back to SQLite in-memory when no real connection string is configured.
/// </summary>
[DependsOn(
  typeof(MaximalHostModule),
  typeof(AbpEntityFrameworkCoreSqlServerModule),
  typeof(AbpEntityFrameworkCoreSqliteModule),
  typeof(AbpTestBaseModule)
)]
public class CrossModuleRealDbTestStartupModule : AbpModule
{
  private static SqliteConnection _fallbackSqliteConnection;

  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    var existingConfig = context.Services.GetConfiguration();
    var configurationBuilder = new ConfigurationBuilder();

    if (existingConfig != null)
    {
      configurationBuilder.AddConfiguration(existingConfig);
    }

    configurationBuilder.AddEnvironmentVariables();

    var overrides = new Dictionary<string, string?>
    {
      { "EventBus:Provider", EventBusProvider.InMemory.ToString() },
    };

    var realDbConnectionString = Environment.GetEnvironmentVariable("ELEON_TEST_REAL_DB_CONNECTION_STRING");
    if (!string.IsNullOrWhiteSpace(realDbConnectionString))
    {
      overrides["ConnectionStrings:Default"] = realDbConnectionString;
    }

    configurationBuilder.AddInMemoryCollection(overrides);

    var configuration = configurationBuilder.Build();
    context.Services.AddSingleton<IConfiguration>(configuration);
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAlwaysDisableUnitOfWorkTransaction();
    context.Services.AddTransient<CurrentEvent>();

    Configure<EventBusOptions>(options =>
    {
      options.Provider = EventBusProvider.InMemory;
      options.ProviderOptionsJson = "{}";
    });

    var configuration = context.Services.GetConfiguration();
    var connectionString = configuration.GetConnectionString("Default");

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
      context.Services.Configure<AbpDbContextOptions>(options =>
      {
        options.Configure(configurationContext =>
        {
          configurationContext.UseSqlServer(
            connectionString,
            sqlOptions => sqlOptions.UseCompatibilityLevel(configuration.GetValue("SqlServer:CompatibilityLevel", 120)));
        });
      });
      return;
    }

    _fallbackSqliteConnection ??= CreateFallbackSqliteConnection();
    context.Services.Configure<AbpDbContextOptions>(options =>
    {
      options.Configure(configurationContext =>
      {
        configurationContext.UseSqlite(_fallbackSqliteConnection);
      });
    });
  }

  private static SqliteConnection CreateFallbackSqliteConnection()
  {
    var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();
    return connection;
  }
}
