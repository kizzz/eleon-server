using Common.Module.Constants;
using Common.Module.Extensions;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.Core.Infrastructure.Module.IdentityServer;

namespace VPortal.Identity.Module.Data
{
  [UnitOfWork]
  public class IdentityApplicationUrlsChangeObserver : ITransientDependency
  {
    private readonly IVportalLogger<IdentityApplicationUrlsChangeObserver> logger;
    private readonly IClientRepository clientRepository;
    private readonly ICurrentTenant currentTenant;
    private readonly IConfiguration configuration;

    public IdentityApplicationUrlsChangeObserver(
        IVportalLogger<IdentityApplicationUrlsChangeObserver> logger,
        IClientRepository clientRepository,
        ICurrentTenant currentTenant,
        IConfiguration configuration)
    {
      this.logger = logger;
      this.clientRepository = clientRepository;
      this.currentTenant = currentTenant;
      this.configuration = configuration;
    }

    public virtual async Task HandleApplicationUrlsChange(Dictionary<VportalApplicationType, List<string>> applicationUrls)
    {
      try
      {
        var clientCfg = configuration.GetSection("IdentityServer").GetSection("Clients").Get<List<IdentitySeedClientDescription>>();
        foreach (var (app, urls) in applicationUrls)
        {
          string clientId = GetClientIdByApp(clientCfg, app);
          if (clientId.NonEmpty())
          {
            await UpdateClientUrls(clientId, urls);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public virtual async Task HandleCacheChange(List<TenantSettingsCacheValue> cache)
    {
      var hostnames = cache.SelectMany(x => x.TenantSetting.Hostnames).ToList();
      var applicationUrls = hostnames.Select(x => (x.ApplicationType, x.Url)).GroupBy(x => x.ApplicationType);
      foreach (var url in applicationUrls)
      {
        var clientCfg = configuration.GetSection("IdentityServer").GetSection("Clients").Get<List<IdentitySeedClientDescription>>();
        string clientId = GetClientIdByApp(clientCfg, url.Key);
        if (clientId.NonEmpty())
        {
          await UpdateClientUrls(clientId, url.Select(x => x.Url).ToList());
        }
      }
    }

    private string GetClientIdByApp(List<IdentitySeedClientDescription> cfg, VportalApplicationType applicationType)
        => cfg.FirstOrDefault(x => x.ApplicationType == applicationType)?.ClientId;

    private async Task UpdateClientUrls(string clientId, List<string> urls)
    {
      var corsOrigins = urls.Select(url => url.TrimEnd('/')).ToList();

      var client = await clientRepository.FindByClientIdAsync(clientId);

      var redirectUrls = urls
          .Select(x => x.TrimEnd('/'))
          .SelectMany<string, string>(x => [x, x + "/ui", x + "/old-ui", x + "/ec/admin", x + "/apps/admin", x + "/apps/erp", x + "/apps/immu", x + "/apps/eleoncore-manager", x + "/apps/elsa", x + "/apps/elsamvc", x + "/server/elsa/signin-oidc"]);

      client.RemoveAllPostLogoutRedirectUris();
      client.RemoveAllRedirectUris();
      client.RemoveAllCorsOrigins();
      foreach (var url in redirectUrls)
      {
        client.AddRedirectUri(url);
        client.AddPostLogoutRedirectUri(url);
      }

      foreach (var origin in corsOrigins)
      {
        client.AddCorsOrigin(origin);
      }

      await clientRepository.UpdateAsync(client);
    }
  }
}
