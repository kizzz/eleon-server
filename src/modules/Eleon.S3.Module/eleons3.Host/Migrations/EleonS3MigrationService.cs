using Common.Module.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Abp.Identity;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
using VPortal.Cms.Feature.Module.EntityFrameworkCore;
using EleonCore.Modules.S3.EntityFrameworkCore;


using Migrations.Module;

namespace VPortal.Migrations;

public class EleonS3MigrationService : IDbMigrationService, ITransientDependency
{
    private readonly IDataSeeder _dataSeeder;
    private readonly ITenantRepository _tenantRepository;
    private readonly List<IVPortalDbSchemaMigrator> _dbSchemaMigrators;
    private readonly IServiceProvider serviceProvider;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly ILogger<EleonS3MigrationService> _logger;

    public EleonS3MigrationService(
        IDataSeeder dataSeeder,
        ITenantRepository tenantRepository,
        EleonS3DbSchemaMigrator unifiedModuleDbSchemaMigrator,
        IServiceProvider serviceProvider,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        ILogger<EleonS3MigrationService> logger)
    {
        _dataSeeder = dataSeeder;
        _tenantRepository = tenantRepository;
        _dbSchemaMigrators = new List<IVPortalDbSchemaMigrator>() { unifiedModuleDbSchemaMigrator };
        this.serviceProvider = serviceProvider;
        _currentTenant = currentTenant;
        this.unitOfWorkManager = unitOfWorkManager;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        if (!StartupArgsParser.IsMigrate && !StartupArgsParser.IsSeed)
        {
            _logger.LogInformation("Automatic migration disabled, skipping migrations");
            return;
        }

        await MigrationCheckHelper.ExecuteMigrationCheckAsync(async result =>
        {
            _logger.LogInformation("Started database migrations...");

            if (StartupArgsParser.IsMigrate)
            {
                await MigrateDatabaseSchemaAsync();
            }
            if (StartupArgsParser.IsSeed)
            {
                await SeedDataAsync();
            }

            _logger.LogInformation($"Successfully completed host database migrations.");

            var tenants = await _tenantRepository.GetListAsync(includeDetails: true);

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
                                var tenantConnectionStrings = tenant.ConnectionStrings
                                    .Select(x => x.Value)
                                    .ToList();

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

                _logger.LogInformation($"Successfully completed {tenant.Name} tenant database migrations.");
            }

            _logger.LogInformation("Successfully completed all database migrations.");
            _logger.LogInformation("You can safely end this process...");
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
            var tenant = await _tenantRepository.GetAsync(id);
            using (_currentTenant.Change(tenant.Id))
            {
                if (tenant.ConnectionStrings.Any())
                {
                    var tenantConnectionStrings = tenant.ConnectionStrings
                        .Select(x => x.Value)
                        .ToList();

                    await MigrateDatabaseSchemaAsync(tenant);
                }

                await SeedDataAsync(tenant, adminEmail, adminPassword, adminUserName);
            }
        }
    }

    private async Task MigrateDatabaseSchemaAsync(Tenant tenant = null)
    {
        _logger.LogInformation(
            $"Migrating schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");

        foreach (var migrator in _dbSchemaMigrators)
        {
            try
            {
                _logger.LogInformation(
                    $"Migrating {migrator.GetType().Name} schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");

                var unifiedDbContext = serviceProvider.GetRequiredService<S3DbContext>();
                await migrator.MigrateAsync();
                _logger.LogInformation(
                    $"Migrated successfully {migrator.GetType().Name} schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Migration errored {migrator.GetType().Name} schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");
                throw;
            }
        }
        _logger.LogInformation(
            $"Migrated schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");
    }

    private async Task SeedDataAsync(
        Tenant tenant = null,
        string adminEmail = MigrationConsts.AdminEmailDefaultValue,
        string adminPassword = MigrationConsts.AdminPasswordDefaultValue,
        string adminUserName = MigrationConsts.AdminUserNameDefaultValue)
    {
        _logger.LogInformation($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database seed...");

        using (var unitOfWork = unitOfWorkManager.Begin(true))
        {
            try
            {
                await _dataSeeder.SeedAsync(new DataSeedContext(tenant?.Id)
                    .WithProperty(IdentityDataSeedContributor.AdminUserNamePropertyName, adminUserName)
                    .WithProperty(IdentityDataSeedContributor.AdminEmailPropertyName, adminEmail)
                    .WithProperty(IdentityDataSeedContributor.AdminPasswordPropertyName, adminPassword)

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
