using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants;
using Logging.Module;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.TenantManagement;
using VPortal.HealthCheckModule.Module;
using VPortal.TenantManagement.Module;

namespace Core.Infrastructure.Module.Tenants;

[Authorize]
public class TenantAppService : IdentityQueryingAppService, Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants.ITenantAppService
{
  private readonly IVportalLogger<TenantAppService> logger;
  private readonly TenantQueryingDomainService domainService;

  public TenantAppService(
      IVportalLogger<TenantAppService> logger,
      TenantQueryingDomainService domainService)
  {
    this.logger = logger;
    this.domainService = domainService;
  }

  public async Task<List<CommonTenantDto>> GetCommonTenantListAsync()
  {
    List<CommonTenantDto> result = new();
    try
    {
      var tenants = await domainService.GetListAsync();
      if (tenants != null)
      {
        foreach (var tenant in tenants)
        {
          var mappedTenant = ObjectMapper.Map<Tenant, CommonTenantDto>(tenant);

          if (tenant.ExtraProperties.TryGetValue("IsRoot", out var isRootValue) && isRootValue is bool isRoot)
          {
            mappedTenant.IsRoot = isRoot;
          }

          result.Add(mappedTenant);
        }
      }
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }

    return result;
  }

  public async Task<CommonTenantDto> GetCommonTenant(Guid tenantId)
  {
    CommonTenantDto result = null;
    try
    {
      var tenant = await domainService.GetTenant(tenantId);

      result = ObjectMapper.Map<Tenant, CommonTenantDto>(tenant);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }

    return result;
  }

  public async Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListAsync()
  {
    List<CommonTenantExtendedDto> result = null;
    try
    {
      var tenants = await domainService.GetListAsync();
      result = await ExtendTenantsAsync(tenants);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }

    return result;
  }

  public async Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListWithCurrentAsync()
  {
    List<CommonTenantExtendedDto> result = null;
    try
    {
      var tenants = await domainService.GetListWithCurrentAsync();
      result = await ExtendTenantsAsync(tenants);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }

    return result;
  }


  private async Task<List<CommonTenantExtendedDto>> ExtendTenantsAsync(List<Tenant> tenants)
  {
    var result = new List<CommonTenantExtendedDto>(tenants.Count);
    foreach (var entity in tenants)
    {
      var tenant = ObjectMapper.Map<Tenant, CommonTenantExtendedDto>(entity);

      try
      {
        if (entity.ExtraProperties.TryGetValue("IsRoot", out var isRootValue) && isRootValue is bool isRoot)
        {
          tenant.IsRoot = isRoot;
        }
        var parentIdUnparsed = entity.ExtraProperties.GetValueOrDefault("ParentId")?.ToString();
        Guid? parentId = parentIdUnparsed.IsNullOrEmpty() ? null : Guid.Parse(parentIdUnparsed);
        tenant.ParentId = parentId;
        result.Add(tenant);
      }
      catch (Exception e)
      {
        e.WithFriendlyMessage($"Failed to fetch configuration of this tenant: {tenant.Name}, {tenant.Id}");
        logger.Capture(e);
      }
    }

    return result;
  }
}
