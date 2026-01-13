using Common.Module.Constants;
using Logging.Module;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.TenantManagement;
using VPortal.TenantManagement.Module.DomainServices;
//using VPortal.TenantManagement.Module.Entities;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.EventHandlers;
public class TenantCreatedHandler : IDistributedEventHandler<EntityCreatedEto<EntityEto>>, ITransientDependency
{
  private readonly TenantDomainService _tenantDomainService;
  //private readonly TenantHostnameDomainService _tenantHostnameDomainService;
  private readonly IVportalLogger<TenantCreatedHandler> logger;

  public TenantCreatedHandler(
      TenantDomainService tenantDomainService,
      //TenantHostnameDomainService tenantHostnameDomainService,
      IVportalLogger<TenantCreatedHandler> logger)
  {
    _tenantDomainService = tenantDomainService;
    //_tenantHostnameDomainService = tenantHostnameDomainService;
    this.logger = logger;
  }

  public async Task HandleEventAsync(EntityCreatedEto<EntityEto> eventData)
  {
    if (eventData.Entity.EntityType == "ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Entities.EleoncoreTenantEntity" && Guid.TryParse(eventData.Entity.KeysAsString, out var tenantId))
    {
      var tenant = await GetTenantAsync(tenantId);
      //var hostname = await CreateDefaultHostnameAsync(tenant);
      //var site = await CreateDefaultSiteAsync(tenant, hostname);
      //var app = await CreateDefaultApplicationAsync(tenant, site);
      // await InitializeConnectionStringsAsync(tenantId);
    }
  }

  private async Task<Tenant> GetTenantAsync(Guid tenantId)
  {
    var tenant = await _tenantDomainService.GetTenant(tenantId);
    return tenant;
  }

  //private async Task InitializeConnectionStringsAsync(Guid tenantId)
  //{
  //    var apps = await _applicationDomainService.GetAllAsync();

  //    foreach (var app in apps)
  //    {
  //        try
  //        {
  //            if (app.UseDedicatedDatabase)
  //            {
  //                var conString = await _connectionStringDomainService.GetAsync(tenantId, app.Name);
  //                if (conString == null)
  //                {
  //                    await _connectionStringDomainService.AddConnectionString(tenantId, app.Name, string.Empty, "INITIALIZING");
  //                }
  //            }
  //        }
  //        catch (Exception ex)
  //        {
  //            logger.Log.LogError(ex, "An error occured when adding initializing connection string for tenant: {tenantId} and application: {appName}", tenantId, app.Name);
  //        }
  //    }
  //}

  //private async Task<TenantHostnameEntity> CreateDefaultHostnameAsync(Tenant tenant)
  //{
  //    return await _tenantHostnameDomainService.AddNonInternalTenantHostname(tenant.Id, "network-protected.com", tenant.Name, false, true, VportalApplicationType.Undefined, true, 443, null); // todo must be internal and use domain from confiuration
  //}

  //private async Task<SiteEntity> CreateDefaultSiteAsync(Tenant tenant, TenantHostnameEntity domain)
  //{
  //    var site = await _siteDomainService.CreateAsync("Administration", [domain.Id], []);
  //    return site;
  //}

  //private async Task<ApplicationEntity> CreateDefaultApplicationAsync(Tenant tenant, SiteEntity site)
  //{
  //    var app = await _applicationDomainService.CreateAsync(
  //        "Administration",
  //        "/apps/admin",
  //        source: "",
  //        isEnabled: true,
  //        isAuthenticationRequired: false,
  //        frameworkType: ClientApplicationFrameworkType.Angular,
  //        styleType: ClientApplicationStyleType.PrimeNg,
  //        clientApplicationType: ClientApplicationType.Portal,
  //        icon: "fa fa-user",
  //        useDedicatedDb: false,
  //        properties: []); // todo is system

  //    await _siteDomainService.UpdateAsync(site.Id, site.Name, site.Hostnames, [app.Id]);

  //    return app;
  //}
}

