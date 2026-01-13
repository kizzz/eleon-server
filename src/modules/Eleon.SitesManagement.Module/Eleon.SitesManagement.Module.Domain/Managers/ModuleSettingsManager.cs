using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftAbp.Messages.Hostnames;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using System.Web;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;
using Location = ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations.Location;

namespace VPortal.SitesManagement.Module.Managers
{
  public class ModuleSettingsManager : DomainService
  {
    private readonly ClientApplicationDomainService clientApplicationDomainService;
    private readonly IDistributedEventBus distributedEventBus;
    private readonly IVportalLogger<ModuleSettingsManager> logger;
    private readonly IObjectMapper objectMapper;
    private readonly ModuleDomainService modulesDomainService;
    private readonly DefaultApplicationsDomainService defaultApplicationsDomainService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEnumerable<string> systemPahtes;
    private readonly string uiSourceUrl;
    private readonly string serverSourceUrl;

    public ModuleSettingsManager(
        ClientApplicationDomainService clientApplicationDomainService,
        IDistributedEventBus distributedEventBus,
        IVportalLogger<ModuleSettingsManager> logger,
        IObjectMapper objectMapper,
        IConfiguration configuration,
        ModuleDomainService modulesDomainService,
        DefaultApplicationsDomainService defaultApplicationsDomainService,
        IHttpContextAccessor httpContextAccessor)
    {
      this.clientApplicationDomainService = clientApplicationDomainService;
      this.distributedEventBus = distributedEventBus;
      this.logger = logger;
      this.objectMapper = objectMapper;
      this.modulesDomainService = modulesDomainService;
      this.defaultApplicationsDomainService = defaultApplicationsDomainService;
      _httpContextAccessor = httpContextAccessor;
      var app = configuration.GetSection("App");

      var domain = app["Domain"];
      var basePath = app["SelfPath"];
      serverSourceUrl = $"https://{domain}{basePath}";
      systemPahtes = app.GetSection("SystemPathes").Get<IEnumerable<string>>() ?? [];
      uiSourceUrl = app["CoreUiSourceUrl"];
    }

    public string ResolveHostname()
    {
      var httpContext = _httpContextAccessor.HttpContext;

      if (httpContext == null)
      {
        throw new Exception("HttpContext is not available.");
      }

      if (httpContext.Request.Headers.TryGetValue("X-Forwarded-Host", out var forwardedHosts))
      {
        return $"https://{forwardedHosts.FirstOrDefault()}";
      }
      if (httpContext.Request.Headers.TryGetValue("host", out var hosts))
      {
        return $"https://{hosts.FirstOrDefault()}";
      }

      if (httpContext.Request.Query.TryGetValue("ReturnUrl", out var redirectUrls))
      {
        var decodedUrl = HttpUtility.UrlDecode(redirectUrls.FirstOrDefault());
        var redirectUrlQuery = HttpUtility.ParseQueryString(decodedUrl);
        var innerRedirectUris = redirectUrlQuery.GetValues("redirect_uri");
        if (innerRedirectUris.Any())
        {
          return HttpUtility.UrlDecode(innerRedirectUris.First());
        }
      }

      if (httpContext.Request.Query.TryGetValue("redirect_uri", out var redirectUris))
      {
        return redirectUris.FirstOrDefault();
      }

      if (httpContext.Request.Headers.TryGetValue("Referer", out var referers))
      {
        string referer = referers.First();
        if (referer.EndsWith('/'))
        {
          referer = referer[..^1];
        }

        return referer;
      }

      if (httpContext.Request.Headers.TryGetValue("Origin", out var origins))
      {
        return origins.FirstOrDefault();
      }

      return null;
    }

    public async Task<ApplicationEntity> GetSiteByHostnameAsync(string hostname)
    {

      try
      {
        var hostnames = await distributedEventBus.RequestAsync<GetTenantHostnamesResponseMsg>(new GetTenantHostnamesRequestMsg { TenantId = CurrentTenant.Id });
        var hostnameEntity = hostnames.Hostnames.FirstOrDefault(h => h.Url == hostname);
        if (hostnameEntity == null || hostnameEntity.AppId == null)
        {
          return null;
        }
        var application = await clientApplicationDomainService.GetAsync(hostnameEntity.AppId.Value);
        return application;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public async Task<ModuleSettingsGotMsg> GetAsync()
    {
      var result = new ModuleSettingsGotMsg();
      try
      {
        var modules = await modulesDomainService.GetByAsync(m => m.IsEnabled);
        result.Modules = objectMapper.Map<List<ModuleEntity>, List<EleoncoreModuleEto>>(modules);
        var clientApps = await clientApplicationDomainService.GetEnabledApplicationsAsync();
        result.ClientApplications = new List<ClientApplicationEto>();
        foreach (var app in clientApps)
        {
          var appDto = objectMapper.Map<ApplicationEntity, ClientApplicationEto>(app);

          //var initRoot = objectMapper.Map<ApplicationModuleEntity, ApplicationModuleEto>(defaultApplicationsDomainService.GetInitRoot());

          ApplicationModuleEntity layoutEntity = defaultApplicationsDomainService.GetLayout(app.StyleType);

          if (layoutEntity == null)
          {
            layoutEntity = app.Modules.FirstOrDefault(m => m.LoadLevel == UiModuleLoadLevel.RootModule);
          }
          var appModules = objectMapper.Map<List<ApplicationModuleEntity>, List<ApplicationModuleEto>>(app.Modules);
          appDto.Modules = [];
          if (layoutEntity != null)
          {

            var layout = objectMapper.Map<ApplicationModuleEntity, ApplicationModuleEto>(layoutEntity);

            appDto.Modules =
            [
                layout,
                            //initRoot,
                        ];

            foreach (var appModule in appModules)
            {
              if (appModule.LoadLevel == UiModuleLoadLevel.SubModule)
              {
                appModule.ParentId = layout.Id;
              }
            }
          }

          appDto.Modules.AddRange(appModules);

          result.ClientApplications.Add(appDto);
        }

        result.TenantId = CurrentTenant.Id;
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

    public async Task<ModuleSettingsGotMsg> GetBySiteIdAsync(Guid siteId)
    {
      var result = new ModuleSettingsGotMsg();
      try
      {
        var site = await clientApplicationDomainService.GetAsync(siteId);

        //var modules = await modulesDomainService.GetByAsync(m => m.IsEnabled);
        //result.Modules = objectMapper.Map<List<ModuleEntity>, List<EleoncoreModuleEto>>(modules);

        var clientApps = await clientApplicationDomainService.GetAllAsync(); // todo enabled
        result.ClientApplications = new List<ClientApplicationEto>();

        clientApps = GetAllSiteAppsRecursively(clientApps, site);
        foreach (var app in clientApps)
        {
          var appDto = objectMapper.Map<ApplicationEntity, ClientApplicationEto>(app);

          //var initRoot = objectMapper.Map<ApplicationModuleEntity, ApplicationModuleEto>(defaultApplicationsDomainService.GetInitRoot());

          ApplicationModuleEntity layoutEntity = defaultApplicationsDomainService.GetLayout(app.StyleType);

          if (layoutEntity == null)
          {
            layoutEntity = app.Modules.FirstOrDefault(m => m.LoadLevel == UiModuleLoadLevel.RootModule);
          }
          var appModules = objectMapper.Map<List<ApplicationModuleEntity>, List<ApplicationModuleEto>>(app.Modules);
          appDto.Modules = [];
          if (layoutEntity != null)
          {

            var layout = objectMapper.Map<ApplicationModuleEntity, ApplicationModuleEto>(layoutEntity);

            appDto.Modules =
            [
                layout,
                            //initRoot,
                        ];

            foreach (var appModule in appModules)
            {
              if (appModule.LoadLevel == UiModuleLoadLevel.SubModule)
              {
                appModule.ParentId = layout.Id;
              }
            }
          }

          appDto.Modules.AddRange(appModules);

          result.ClientApplications.Add(appDto);
        }

        result.TenantId = CurrentTenant.Id;
        result.SiteId = siteId;
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

    private List<ApplicationEntity> GetAllSiteAppsRecursively(List<ApplicationEntity> apps, ApplicationEntity parent, List<ApplicationEntity> result = null, int deepth = 0)
    {
      if (deepth > 100000)
      {
        throw new Exception("Failed to collect apps for site");
      }

      result ??= new List<ApplicationEntity>();

      foreach (var app in apps.Where(x => x.ParentId == parent.Id))
      {
        result.Add(app);
        GetAllSiteAppsRecursively(apps, app, result, deepth + 1);
      }

      return result;
    }

    public async Task RefreshAsync()
    {

      try
      {
        var settings = await GetAsync();
        var msg = new ModuleSettingsRefreshedMsg
        {
          TenantId = settings.TenantId,
          ClientApplications = settings.ClientApplications,
          Modules = settings.Modules,
          SiteId = settings.SiteId,
          TenantName = settings.TenantName,
        };
        await distributedEventBus.PublishAsync(msg);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    private List<Location> CollectSiteSublocations(List<ApplicationEntity> apps, ApplicationEntity app, string defaultRedirect, int deepth = 0)
    {
      if (deepth > 100000)
      {
        logger.Log.LogCritical("Failed to collect locations for app {appName} ({appId})", app.Name, app.Id);
        throw new Exception($"Failed to collect locations for app {app.Name} ({app.Id})");
      }

      var children = apps.Where(x => x.ParentId == app.Id);

      var locations = new List<Location>();

      foreach (var child in children)
      {
        locations.Add(new Location
        {
          Path = child.Path.StartsWith('/') ? child.Path.Substring(1) : child.Path,
          DefaultRedirect = defaultRedirect,
          SourceUrl = GetSourceUrl(child.FrameworkType, child.Source),
          Type = MapType(child.FrameworkType),
          ResourceId = child.Id.ToString(),
          SubLocations = CollectSiteSublocations(apps, child, defaultRedirect, deepth + 1)
        });
      }

      return locations;
    }

    public async Task<List<Location>> GetLocationsBySiteIdAsync(Guid siteId)
    {
      var apps = await clientApplicationDomainService.GetAllAsync();
      var site = await clientApplicationDomainService.GetAsync(siteId);
      apps = GetAllSiteAppsRecursively(apps, site);

      var defaultRedirectUrl = "/apps/admin"; // await GetDefaultRedirectPath(apps); // Todo from site

      var systemLocations = systemPahtes.Select(p => new Location
      {
        Path = p.StartsWith('/') ? p.Substring(1) : p,
        DefaultRedirect = defaultRedirectUrl,
        SourceUrl = serverSourceUrl + p.EnsureStartsWith('/'),
        Type = LocationType.Other,
      }).ToList();

      var locations = new List<Location>();
      locations.AddRange(systemLocations);
      locations.Add(new Location()
      {
        Path = "modules/layout",
        Type = LocationType.Other,
        DefaultRedirect = defaultRedirectUrl,
        SourceUrl = "https://localhost:1010/primeng-layout"
      });
      locations.AddRange(CollectSiteSublocations(apps, site, defaultRedirectUrl));

      return new List<Location>
            {
                new Location
                {
                    Path = null,
                    Type = LocationType.Virtual,
                    DefaultRedirect = defaultRedirectUrl,
                    SubLocations = locations
                }
            };
    }

    public async Task<List<Location>> GetLocationsAsync()
    {

      try
      {
        var settings = await GetAsync();

        var modules = settings.Modules;
        var clientApps = settings.ClientApplications;

        var defaultRedirectUrl = await GetDefaultRedirectPath(clientApps);

        var locations = systemPahtes.Select(p => new Location
        {
          Path = p.StartsWith('/') ? p.Substring(1) : p,
          DefaultRedirect = defaultRedirectUrl,
          SourceUrl = serverSourceUrl + p.EnsureStartsWith('/'),
          Type = LocationType.Other,
        }).ToList();

        locations.Add(new Location
        {
          Path = "apps",
          Type = LocationType.Virtual,
          DefaultRedirect = defaultRedirectUrl,
          SubLocations = clientApps
                .Select(a => new Location
                {
                  Path = ValidateAppsPath(a.Path),
                  Type = MapType(a.FrameworkType),
                  SourceUrl = GetSourceUrl(a),
                  ResourceId = a.Id.ToString(),
                  DefaultRedirect = defaultRedirectUrl
                })
                .ToList()
        });
        locations.Add(new Location
        {
          Path = "modules",
          Type = LocationType.Virtual,
          SubLocations = modules
                .Where(m => m.Type == ModuleType.Client)
                .Select(m => new Location
                {
                  Path = ValidateClientModulesPath(m.Path),
                  Type = LocationType.Other,
                  ResourceId = m.Id.ToString(),
                  SourceUrl = m.Source,
                  DefaultRedirect = defaultRedirectUrl
                })
                .ToList()
        });

        locations.AddRange(modules
                .Where(m => m.Type != ModuleType.Client)
                .Select(m => new Location
                {
                  Path = ValidateModulesPath(m.Path),
                  Type = LocationType.Other,
                  ResourceId = m.Id.ToString(),
                  SourceUrl = m.Source,
                  DefaultRedirect = defaultRedirectUrl
                }));

        return new List<Location>
                {
                    new Location
                    {
                        Path = null,
                        Type = LocationType.Virtual,
                        DefaultRedirect = defaultRedirectUrl,
                        SubLocations = locations
                    }
                };
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return null;
    }

    private string GetSourceUrl(ClientApplicationEto a)
    {
      return GetSourceUrl(a.FrameworkType, a.Source);
    }

    private string GetSourceUrl(ClientApplicationFrameworkType type, string appSource)
    {
      if (type == ClientApplicationFrameworkType.Angular || type == ClientApplicationFrameworkType.React)
      {
        return uiSourceUrl + "/host";
      }
      return appSource;
    }

    private async Task<string> GetDefaultRedirectPath(IEnumerable<ClientApplicationEto> apps)
    {
      var defaultApp = await clientApplicationDomainService.GetDefaultApplicationAsync();
      return _httpContextAccessor.HttpContext.Request.PathBase + (defaultApp?.Path ?? "/apps/admin").EnsureStartsWith('/');
    }


    private static LocationType MapType(ClientApplicationFrameworkType frameworkType)
    {
      if (frameworkType == ClientApplicationFrameworkType.Angular)
      {
        return LocationType.Angular;
      }

      if (frameworkType == ClientApplicationFrameworkType.CustomAngular)
      {
        return LocationType.Other;
      }

      return LocationType.Other;
    }

    private static string ValidateClientModulesPath(string path)
    {
      path = path.Trim().Trim('/');

      if (path.StartsWith("modules"))
      {
        path = path.ReplaceFirst("modules", "");
      }

      return path.Trim().Trim('/');
    }
    private static string ValidateModulesPath(string path)
    {
      return path.Trim().Trim('/');
    }


    private static string ValidateAppsPath(string path)
    {
      path = path.Trim().Trim('/');

      if (path.StartsWith("apps"))
      {
        path = path.ReplaceFirst("apps", "");
      }

      return path.Trim().Trim('/');
    }
  }
}


