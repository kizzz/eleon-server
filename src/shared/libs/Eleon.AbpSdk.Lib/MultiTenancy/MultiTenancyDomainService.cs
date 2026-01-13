using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedModule.modules.MultiTenancy.Module;
using TenantSettings.Module.Cache;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;

namespace TenantSettings.Module.Helpers
{
  /// <summary>
  /// A DomainService that provides helper methods for working with multi-tenancy.
  /// </summary>
  public class MultiTenancyDomainService : DomainService
  {
    private readonly ILogger<MultiTenancyDomainService> logger;
    private readonly IDistributedEventBus requestClient;
    private readonly TenantCacheService _tenantCacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiTenancyDomainService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="tenantRepository">The tenant repository.</param>
    public MultiTenancyDomainService(
            ILogger<MultiTenancyDomainService> logger,
            IDistributedEventBus requestClient,
            TenantCacheService tenantCacheService)
    {
      this.logger = logger;
      this.requestClient = requestClient;
      _tenantCacheService = tenantCacheService;
    }

    /// <summary>
    /// Executes the given function for the host tenant, and then for each tenant in the system.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A list of results, one for host and one for each tenant.</returns>
    public async Task<List<TResult>> ForEachTenant<TResult>(Func<Task<TResult>> func, bool activeTenantsOnly = true)
    {
      var results = new List<TResult>();
      try
      {
        await DoForAllTenants(async tenantId => results.Add(await func()), activeTenantsOnly);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred while processing the request.");
      }

      return results;
    }

    /// <summary>
    /// Executes the given function for the host tenant, and then for each tenant in the system.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A list of results, one for host and one for each tenant.</returns>
    public async Task<List<TResult>> ForEachTenant<TResult>(Func<Guid?, Task<TResult>> func, bool activeTenantsOnly = true)
    {
      var results = new List<TResult>();
      try
      {
        await DoForAllTenants(async tenantId => results.Add(await func(tenantId)), activeTenantsOnly);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred while processing the request.");
      }

      return results;
    }

    /// <summary>
    /// Executes the given function for the host tenant, and then for each tenant in the system, collecting the results.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A list of results, one for host and one for each tenant.</returns>
    public async Task<List<TResult>> CollectForAllTenants<TResult>(Func<Guid?, Task<List<TResult>>> func, bool activeTenantsOnly = true)
    {
      var results = new List<TResult>();
      try
      {
        await DoForAllTenants(async tenantId => results.AddRange(await func(tenantId)), activeTenantsOnly);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred while processing the request.");
      }

      return results;
    }

    /// <summary>
    /// Executes the given function for the host tenant, and then for each tenant in the system.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <returns>A list of results, one for host and one for each tenant.</returns>
    public async Task ForEachTenant(Func<Guid?, Task> func, bool activeTenantsOnly = true)
    {
      try
      {
        await DoForAllTenants(func, activeTenantsOnly);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred while processing the request.");
      }
    }

    private async Task DoForAllTenants(Func<Guid?, Task> action, bool activeTenantsOnly = true)
    {
      await DoForTenant(null, null, action);

      var allTenants = await GetTenants(activeTenantsOnly);
      foreach (var tenant in allTenants)
      {
        await DoForTenant(tenant.Id, tenant.Name, action);
      }
    }

    private async Task DoForTenant(Guid? tenantId, string tenantName, Func<Guid?, Task> action)
    {
      using (CurrentTenant.Change(tenantId))
      {
        bool isHost = tenantId == null;
        string tenantInfo = isHost ? "host tenant" : $"tenant {tenantName} ({tenantId})";
        try
        {
          logger.LogDebug($"Running MultiTenancyDomainService.ForEachTenant for {tenantInfo}");
          await action(tenantId);
          logger.LogDebug($"Finished MultiTenancyDomainService.ForEachTenant for {tenantInfo}");
        }
        catch (Exception ex)
        {
          var wrapped = new Exception($"An error occured while performing task for {tenantInfo}. See inner exception for details.", ex);
          if (isHost)
          {
            logger.LogError(wrapped, "An error occurred while processing the request.");
          }
          else
          {
            logger.LogError(wrapped, "An error occurred while processing the request.");
          }
        }
      }
    }

    private async Task<List<TenantEto>> GetTenants(bool activeOnly)
    {
      //var request = new GetAllTenantsMsg();
      //var response = await requestClient.RequestAsync<AllTenantsGotMsg>(request);

      var tenants = await _tenantCacheService.GetTenantsAsync();

      if (activeOnly)
      {
        return tenants.Where(x => x.Status == TenantStatus.Active).ToList();
      }

      return tenants;
    }
  }
}
