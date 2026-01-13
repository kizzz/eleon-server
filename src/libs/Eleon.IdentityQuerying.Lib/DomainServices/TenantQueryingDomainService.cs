using Common.EventBus.Module;
using Eleon.InternalCommons.Lib.Messages.Hostnames;
using Eleon.InternalCommons.Lib.Messages.Identity;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Migrations.Module;
using TenantSettings.Module.Cache;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;


public class TenantQueryingDomainService : DomainService
{
  private readonly IVportalLogger<TenantQueryingDomainService> logger;
  private readonly ITenantRepository tenantRepository;

  public TenantQueryingDomainService(IServiceProvider serviceProvider)
  {
    this.logger = serviceProvider.GetRequiredService<IVportalLogger<TenantQueryingDomainService>>();
    this.tenantRepository = serviceProvider.GetRequiredService<ITenantRepository>();
  }

  public async Task<Tenant> GetTenant(Guid tenantId)
  {
    Tenant result = null;
    try
    {
      result = await tenantRepository.GetAsync(tenantId);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }

  public async Task<List<Tenant>> GetListWithCurrentAsync()
  {
    List<Tenant> result = null;
    try
    {
      var currentTenantId = CurrentTenant.Id;
      if (!currentTenantId.HasValue)
      {
        return await GetListAsync();
      }

      result = await GetListAsync();
      var currentTenant = result.FirstOrDefault(t => t.Id == currentTenantId.Value);

      if (currentTenant == null)
      {
        using (CurrentTenant.Change(null))
        {
          var tenant = await tenantRepository.GetAsync(currentTenantId.Value);
          result.Add(tenant);
        }
      }

    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }

  public async Task<List<Tenant>> GetListAsync()
  {
    List<Tenant> result = null;
    try
    {
      result = await tenantRepository.GetListAsync(includeDetails: true);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }  
}
