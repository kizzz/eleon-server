using Common.Module.Extensions;
using Microsoft.Extensions.Options;
using TenantSettings.Module.Cache;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace Authorization.Module.MicroserviceInitialization
{
  // DEPRECATED
  //[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  public class MicroserviceConnectionStringResolver : DefaultConnectionStringResolver
  {
    private readonly ICurrentTenant _currentTenant;
    private readonly IServiceProvider _serviceProvider;

    public MicroserviceConnectionStringResolver(
        IOptionsMonitor<AbpDbConnectionOptions> options,
        ICurrentTenant currentTenant,
        IServiceProvider serviceProvider)
        : base(options)
    {
      _currentTenant = currentTenant;
      _serviceProvider = serviceProvider;
    }

    public override async Task<string> ResolveAsync(string connectionStringName = null)
    {
      if (_currentTenant.Id == null)
      {
        //No current tenant, fallback to default logic
        return await base.ResolveAsync(connectionStringName);
      }

      var tenant = await FindTenantConfigurationAsync(_currentTenant.Id.Value);

      if (tenant == null || tenant.ConnectionStrings.IsNullOrEmpty())
      {
        //Tenant has not defined any connection string, fallback to default logic
        return await base.ResolveAsync(connectionStringName);
      }

      var tenantDefaultConnectionString = tenant.ConnectionStrings?.Default;

      //Requesting default connection string...
      if (connectionStringName == null ||
          connectionStringName == ConnectionStrings.DefaultConnectionStringName)
      {
        //Return tenant's default or global default
        return !tenantDefaultConnectionString.IsNullOrWhiteSpace()
            ? tenantDefaultConnectionString!
            : Options.ConnectionStrings.Default!;
      }

      //Requesting specific connection string...
      var connString = tenant.ConnectionStrings?.GetOrDefault(connectionStringName);
      if (!connString.IsNullOrWhiteSpace())
      {
        //Found for the tenant
        return connString!;
      }

      //Fallback to the mapped database for the specific connection string
      var database = Options.Databases.GetMappedDatabaseOrNull(connectionStringName);
      if (database != null && database.IsUsedByTenants)
      {
        connString = tenant.ConnectionStrings?.GetOrDefault(database.DatabaseName);
        if (!connString.IsNullOrWhiteSpace())
        {
          //Found for the tenant
          return connString!;
        }
      }

      //Fallback to tenant's default connection string if available
      if (!tenantDefaultConnectionString.IsNullOrWhiteSpace())
      {
        return tenantDefaultConnectionString!;
      }

      return await base.ResolveAsync(connectionStringName);
    }

    [Obsolete("Use ResolveAsync method.")]
    public override string Resolve(string connectionStringName = null)
    {
      if (_currentTenant.Id == null)
      {
        //No current tenant, fallback to default logic
        return base.Resolve(connectionStringName);
      }

      var tenant = FindTenantConfigurationAsync(_currentTenant.Id.Value).Result;

      if (tenant == null || tenant.ConnectionStrings.IsNullOrEmpty())
      {
        //Tenant has not defined any connection string, fallback to default logic
        return base.Resolve(connectionStringName);
      }

      var tenantDefaultConnectionString = tenant.ConnectionStrings?.Default;

      //Requesting default connection string...
      if (connectionStringName == null ||
          connectionStringName == ConnectionStrings.DefaultConnectionStringName)
      {
        //Return tenant's default or global default
        return !tenantDefaultConnectionString.IsNullOrWhiteSpace()
            ? tenantDefaultConnectionString!
            : Options.ConnectionStrings.Default!;
      }

      //Requesting specific connection string...
      var connString = tenant.ConnectionStrings?.GetOrDefault(connectionStringName);
      if (!connString.IsNullOrWhiteSpace())
      {
        //Found for the tenant
        return connString!;
      }

      //Fallback to the mapped database for the specific connection string
      var database = Options.Databases.GetMappedDatabaseOrNull(connectionStringName);
      if (database != null && database.IsUsedByTenants)
      {
        connString = tenant.ConnectionStrings?.GetOrDefault(database.DatabaseName);
        if (!connString.IsNullOrWhiteSpace())
        {
          //Found for the tenant
          return connString!;
        }
      }

      //Fallback to tenant's default connection string if available
      if (!tenantDefaultConnectionString.IsNullOrWhiteSpace())
      {
        return tenantDefaultConnectionString!;
      }

      return base.Resolve(connectionStringName);
    }

    protected virtual async Task<TenantConfiguration?> FindTenantConfigurationAsync(Guid tenantId)
    {
      // Retrieve the TenantCacheService from the service provider
      var tenantSettingsCache = _serviceProvider.GetService<TenantCacheService>();

      if (tenantSettingsCache == null)
      {
        throw new InvalidOperationException("TenantCacheService is not registered.");
      }

      // Find the tenant from the cache
      var tenant = (await tenantSettingsCache
          .GetTenantsAsync())
          .FirstOrDefault(t => t.Id == tenantId);

      if (tenant == null)
      {
        return null; // Return null if tenant is not found
      }

      // Create and populate the TenantConfiguration
      var tenantConfiguration = new TenantConfiguration(tenant.Id, tenant.Name)
      {
        ConnectionStrings = new ConnectionStrings()
      };

      foreach (var connectionString in tenant.ConnectionStrings)
      {
        tenantConfiguration.ConnectionStrings[connectionString.Name] = connectionString.Value;
      }

      return tenantConfiguration;
    }

  }
}
