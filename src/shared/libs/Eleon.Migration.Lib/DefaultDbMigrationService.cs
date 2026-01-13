using Common.Module.Migrations;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
using Messaging.Module.ETO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migrations.Module;
using SharedModule.modules.MultiTenancy.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal;

namespace EleonsoftSdk.modules.Migration.Module;

public class DefaultDbMigrationService : IDbMigrationService
{
  private readonly IDataSeeder _dataSeeder;
  private readonly List<IVPortalDbSchemaMigrator> _dbSchemaMigrators;
  private readonly ICurrentTenant _currentTenant;
  private readonly IUnitOfWorkManager unitOfWorkManager;
  private readonly ILogger<DefaultDbMigrationService> _logger;
  private readonly IConfiguration _configuration;

  public DefaultDbMigrationService(
      IServiceProvider serviceProvider,
      IDataSeeder dataSeeder,
      IEnumerable<IVPortalDbSchemaMigrator> dbSchemaMigrators)
  {
    _dataSeeder = dataSeeder;
    _dbSchemaMigrators = new List<IVPortalDbSchemaMigrator>(dbSchemaMigrators);
    _currentTenant = serviceProvider.GetRequiredService<ICurrentTenant>();
    this.unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
    _logger = serviceProvider.GetRequiredService<ILogger<DefaultDbMigrationService>>();
    _configuration = serviceProvider.GetRequiredService<IConfiguration>();
  }

  public async Task MigrateAsync()
  {
    if (!StartupArgsParser.IsMigrate && !StartupArgsParser.IsSeed)
    {
      _logger.LogDebug("Automatic migration disabled, skipping migrations");
      return;
    }

    await MigrationCheckHelper.ExecuteMigrationCheckAsync(async result =>
    {
      _logger.LogDebug("Started database migrations...");

      if (StartupArgsParser.IsMigrate)
      {
        await MigrateDatabaseSchemaAsync();
      }
      if (StartupArgsParser.IsSeed)
      {
        await SeedDataAsync();
      }

      _logger.LogDebug($"Successfully completed host database migrations.");

      if (_configuration.GetValue($"{EleonMultiTenancyOptions.DefaultSectionName}:{nameof(EleonMultiTenancyOptions.Enabled)}", true))
      {
        var tenants = (await GetTenantsAsync());

        var migratedDatabaseSchemas = new HashSet<string>();

        foreach (var tenant in tenants)
        {
          try
          {
            using (_currentTenant.Change(tenant.Id))
            {
              if (StartupArgsParser.IsMigrate)
              {
                if (tenant.ConnectionStrings.Any())
                {
                  var tenantConnectionStrings = tenant.ConnectionStrings.ToList();

                  if (!migratedDatabaseSchemas.IsSupersetOf(tenantConnectionStrings))
                  {
                    await MigrateDatabaseSchemaAsync(tenant);

                    migratedDatabaseSchemas.AddIfNotContains(tenantConnectionStrings);
                  }
                }
              }

              if (StartupArgsParser.IsSeed)
              {
                await SeedDataAsync(tenant);
              }
            }
          }
          catch (Exception ex)
          {
            _logger.LogError($"Error with migration {ex.Message} on {tenant.Name} ({tenant.Id})");
            throw;
          }

          _logger.LogDebug($"Successfully completed {tenant.Name} tenant database migrations.");
        }
      }

      _logger.LogDebug("Successfully completed all database migrations.\nYou can safely end this process...");
    });
  }

  public async Task MigrateTenantAsync(
      Guid id,
      string adminEmail = MigrationConsts.AdminEmailDefaultValue,
      string adminPassword = MigrationConsts.AdminPasswordDefaultValue,
      string adminUserName = MigrationConsts.AdminUserNameDefaultValue)
  {
    using (_currentTenant.Change(null))
    {
      var tenant = (await GetTenantsAsync()).Single(x => x.Id == id);
      using (_currentTenant.Change(tenant.Id))
      {
        if (tenant.ConnectionStrings.Any())
        {
          var tenantConnectionStrings = tenant.ConnectionStrings.ToList();

          await MigrateDatabaseSchemaAsync(tenant);
        }

        await SeedDataAsync(tenant, adminEmail, adminPassword, adminUserName);
      }
    }
  }

  private async Task MigrateDatabaseSchemaAsync(TenantInfo tenant = null)
  {
    var tenantName = (tenant == null ? "host" : tenant.Name + " tenant");
    _logger.LogDebug(
        $"Migrating schema for {tenantName} database...");

    var exceptions = new List<Exception>();

    foreach (var migrator in _dbSchemaMigrators)
    {
      try
      {
        _logger.LogDebug(
            $"Migrating {migrator.GetType().Name} schema for {tenantName} database...");

        await migrator.MigrateAsync();
        _logger.LogDebug(
            $"Migrated successfully {migrator.GetType().Name} schema for {tenantName} database...");
      }
      catch (Exception ex)
      {
        _logger.LogError(
            $"Migration errored {migrator.GetType().Name} schema for {tenantName} database...");
        exceptions.Add(ex);
      }
    }

    if (exceptions.Any())
    {
      throw new AggregateException(
          $"One or more errors occurred while migrating the database schema for {tenantName} database.",
          exceptions);
    }

    _logger.LogDebug($"Successfully migrated schema for {tenantName} database");
  }

  private async Task SeedDataAsync(
      TenantInfo tenant = null,
      string adminEmail = MigrationConsts.AdminEmailDefaultValue,
      string adminPassword = MigrationConsts.AdminPasswordDefaultValue,
      string adminUserName = MigrationConsts.AdminUserNameDefaultValue)
  {
    _logger.LogDebug($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database seed...");

    using (var unitOfWork = unitOfWorkManager.Begin(true))
    {
      try
      {
        await _dataSeeder.SeedAsync(new DataSeedContext(tenant?.Id)
            .WithProperty(MigrationConsts.AdminUserNamePropertyName, adminUserName)
            .WithProperty(MigrationConsts.AdminEmailPropertyName, adminEmail)
            .WithProperty(MigrationConsts.AdminPasswordPropertyName, adminPassword)

        );
        await unitOfWork.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(
            $"Seed for {(tenant == null ? "host" : tenant.Name + " tenant")} database: {ex.Message}...");
        throw;
      }
    }
  }

  public record TenantInfo(Guid Id, string Name, List<string> ConnectionStrings);
  public virtual async Task<List<TenantInfo>> GetTenantsAsync()
  {
    return new List<TenantInfo>();
  }

  private static bool DoesTableExist(DbContext context, string tableName)
  {
    // Ensure the database connection is open
    var connection = context.Database.GetDbConnection();

    if (connection.State != System.Data.ConnectionState.Open)
    {
      connection.Open();
    }

    // Use the query specific to your database system
    // For SQL Server:
    var command = connection.CreateCommand();
    command.CommandText = $@"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = @TableName
                ) THEN 1 ELSE 0 END";

    var parameter = command.CreateParameter();
    parameter.ParameterName = "@TableName";
    parameter.Value = tableName;
    command.Parameters.Add(parameter);

    // Execute the command and return the result
    var result = (int)command.ExecuteScalar();
    return result == 1;
  }
}
