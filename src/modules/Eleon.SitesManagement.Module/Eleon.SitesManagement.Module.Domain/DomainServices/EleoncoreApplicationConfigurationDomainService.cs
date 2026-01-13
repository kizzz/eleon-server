using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftAbp.Messages.AppConfig;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Configuration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Repositories;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices
{
  public class EleoncoreApplicationConfigurationDomainService : DomainService
  {
    private readonly IVportalLogger<EleoncoreApplicationConfigurationDomainService> logger;
    private readonly AbpApplicationConfigurationAppService abpApplicationConfigurationAppService;
    private readonly ModuleSettingsManager moduleSettingsManager;
    private readonly IConfiguration configuration;
    private readonly IClientApplicationRepository clientApplicationRepository;
    private readonly IDistributedEventBus _eventBus;

    public EleoncoreApplicationConfigurationDomainService(
        IVportalLogger<EleoncoreApplicationConfigurationDomainService> logger,
        AbpApplicationConfigurationAppService abpApplicationConfigurationAppService,
        ModuleSettingsManager moduleSettingsManager,
        IConfiguration configuration,
        IClientApplicationRepository clientApplicationRepository,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.abpApplicationConfigurationAppService = abpApplicationConfigurationAppService;
      this.moduleSettingsManager = moduleSettingsManager;
      this.configuration = configuration;
      this.clientApplicationRepository = clientApplicationRepository;
      _eventBus = eventBus;
    }

    public async Task<EleoncoreApplicationConfigurationValueObject> GetAsync(string path)
    {
      EleoncoreApplicationConfigurationValueObject result = null;
      try
      {
        var settings = await moduleSettingsManager.GetAsync();

        var clientApps = settings.ClientApplications;

        var application = clientApps
            .FirstOrDefault(app => path == app.Path);

        if (application == null)
        {
          throw new UserFriendlyException($"App not found for {application.Path}");
        }

        var response = await _eventBus.RequestAsync<GetBaseAppConfigResponseMsg>(new GetBaseAppConfigRequestMsg());

        if (!response.Success)
        {
          throw new Exception("Faild to get base app config from event bus");
        }

        var applicationConfigurationDto = response.ApplicationConfiguration; // await abpApplicationConfigurationAppService.GetAsync(new ApplicationConfigurationRequestOptions() { IncludeLocalizationResources = false });

        result = new EleoncoreApplicationConfigurationValueObject
        {
          Localization = applicationConfigurationDto.Localization,
          Auth = applicationConfigurationDto.Auth,
          CurrentUser = applicationConfigurationDto.CurrentUser,
          Features = applicationConfigurationDto.Features,
          CurrentTenant = applicationConfigurationDto.CurrentTenant,
          ExtraProperties = applicationConfigurationDto.ExtraProperties,
          CorePath = applicationConfigurationDto.CorePath ?? "/core",
          AuthPath = applicationConfigurationDto.AuthPath ?? "/auth",
          OAuthConfig = applicationConfigurationDto.OAuthConfig ?? new OAuthConfigValueObject
          {
            ClientId = "VPortal_App",
            ResponseType = "code",
            Scope = "openid profile offline_access VPortal",
            UseSilentRefresh = false,
          },
          WebPush = applicationConfigurationDto.WebPush ?? new WebPushConfigValueObject
          {
            PublicKey = string.Empty,
          },
          Production = false,
          ApplicationName = application.Name,
          ApplicationPath = application.Path,
          FrameworkType = application.FrameworkType.ToString(),
          StyleType = application.StyleType.ToString(),
          ClientApplicationType = application.ClientApplicationType.ToString(),
          ClientApplication = application,
          Modules = application.Modules,
        };

        result.Localization.CurrentCulture = CurrentCultureDto.Create();
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


    private static ClientApplicationEto ResolveApplicationByPath(Guid siteId, List<ClientApplicationEto> allApps, string path)
    {
      var fullPathCache = new Dictionary<Guid, string>();
      ClientApplicationEto bestMatch = null;
      string longestFullPath = null;

      foreach (var app in allApps)
      {
        string fullPath = BuildFullPath(siteId, app, allApps, fullPathCache, 0);

        if (!string.IsNullOrEmpty(fullPath) &&
            path.Contains(fullPath, StringComparison.OrdinalIgnoreCase) &&
            fullPath.Length > (longestFullPath?.Length ?? 0))
        {
          bestMatch = app;
          longestFullPath = fullPath;
        }
      }

      return bestMatch ?? throw new Exception("Application not resolved");
    }

    private static string BuildFullPath(Guid siteId,
                                        ClientApplicationEto app,
                                        List<ClientApplicationEto> allApps,
                                        Dictionary<Guid, string> cache,
                                        int depth)
    {
      if (depth > 100_000)
        throw new InvalidOperationException("Exceeded maximum depth while building full path (possible cycle in app tree)");

      if (cache.TryGetValue(app.Id, out var cached))
        return cached;

      string fullPath = app.Path?.Trim('/') ?? "";

      if (app.ParentId != null && app.ParentId != siteId)
      {
        var parent = allApps.FirstOrDefault(a => a.Id == app.ParentId);
        if (parent == null)
        {
          return null;
        }

        var parentPath = BuildFullPath(siteId, parent, allApps, cache, depth + 1);
        if (string.IsNullOrEmpty(parentPath))
          return null;

        fullPath = $"{parentPath}/{fullPath}".Trim('/');
      }

      fullPath = fullPath.EnsureStartsWith('/');
      cache[app.Id] = fullPath;
      return fullPath;
    }

    public async Task<EleoncoreApplicationConfigurationValueObject> GetBySiteAsync(string path)
    {
      EleoncoreApplicationConfigurationValueObject result = null;
      try
      {
        // 1. resolve hostname
        var hostname = moduleSettingsManager.ResolveHostname();

        // 2. resolve site by hostname
        var site = await moduleSettingsManager.GetSiteByHostnameAsync(hostname);

        // 3. resolve site applications
        var settings = await moduleSettingsManager.GetBySiteIdAsync(site.Id);
        var clientApps = settings.ClientApplications;

        // 4. resolve application by path
        var application = ResolveApplicationByPath(site.Id, clientApps, path);

        var modules = clientApps.Where(x => x.ParentId == application.Id && x.AppType == Commons.Module.Proxy.Constants.ApplicationType.Module).ToList();

        if (application == null)
        {
          throw new UserFriendlyException($"App not found for {application.Path}");
        }
        var authPath = configuration["ProxyRouter:AuthPath"];
        var corePath = configuration["ProxyRouter:CorePath"];
        var webPush = configuration["ProxyRouter:WebPush"];

        var response = await _eventBus.RequestAsync<GetBaseAppConfigResponseMsg>(new GetBaseAppConfigRequestMsg());

        if (!response.Success)
        {
          throw new Exception("Faild to get base app config from event bus");
        }

        var applicationConfigurationDto = response.ApplicationConfiguration; // await abpApplicationConfigurationAppService.GetAsync(new ApplicationConfigurationRequestOptions() { IncludeLocalizationResources = false });

        result = new EleoncoreApplicationConfigurationValueObject
        {
          Localization = applicationConfigurationDto.Localization,
          Auth = applicationConfigurationDto.Auth,
          CurrentUser = applicationConfigurationDto.CurrentUser,
          Features = applicationConfigurationDto.Features,
          CurrentTenant = applicationConfigurationDto.CurrentTenant,
          ExtraProperties = applicationConfigurationDto.ExtraProperties,
          Production = false,
          ApplicationName = application.Name,
          ApplicationPath = application.Path,
          FrameworkType = application.FrameworkType.ToString(),
          StyleType = application.StyleType.ToString(),
          ClientApplicationType = application.ClientApplicationType.ToString(),
          ClientApplication = application,
          Modules = modules.Select(x => new ApplicationModuleEto
          {
            Id = x.Id,
            ClientApplicationEntityId = x.ParentId.Value,
            Name = x.Name,
            Expose = x.Expose,
            LoadLevel = Enum.Parse<UiModuleLoadLevel>(x.LoadLevel),
            OrderIndex = x.OrderIndex,
            PluginName = x.Name,
            ParentId = x.ParentId,
            Properties = x.Properties,
            Url = application.Path + x.Path.EnsureStartsWith('/')
          }).ToList(),
          CorePath = corePath,
          AuthPath = authPath,
          OAuthConfig = new OAuthConfigValueObject
          {
            ClientId = "VPortal_App",
            ResponseType = "code",
            Scope = "openid profile offline_access VPortal",
            UseSilentRefresh = false
          },
          WebPush = new WebPushConfigValueObject
          {
            PublicKey = webPush,
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
      return result;

    }
  }
}
