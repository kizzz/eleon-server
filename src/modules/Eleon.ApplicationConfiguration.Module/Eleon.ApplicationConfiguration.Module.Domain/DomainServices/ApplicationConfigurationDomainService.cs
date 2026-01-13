using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftAbp.Messages.AppConfig;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System.Collections.Concurrent;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace VPortal.ApplicationConfiguration.Module.DomainServices;


public class ApplicationConfigurationDomainService : DomainService
{
  private readonly IVportalLogger<ApplicationConfigurationDomainService> _logger;
  private readonly IDistributedEventBus _eventBus;
  private readonly IMemoryCache _cache;
  private readonly IConfiguration _configuration;
  private readonly AbpApplicationConfigurationAppService _abpApplicationConfigurationAppService;
  private readonly ICurrentUser _currentUser;

  public ApplicationConfigurationDomainService(
          IVportalLogger<ApplicationConfigurationDomainService> logger,
          IDistributedEventBus eventBus,
          IMemoryCache cache,
          IConfiguration configuration,
          AbpApplicationConfigurationAppService abpApplicationConfigurationAppService,
          ICurrentUser currentUser
)
  {
    _logger = logger;
    _eventBus = eventBus;
    _cache = cache;
    _configuration = configuration;
    _abpApplicationConfigurationAppService = abpApplicationConfigurationAppService;
    _currentUser = currentUser;
  }

  public async Task<EleoncoreApplicationConfigurationValueObject> GetAsync(string applicationId)
  {
    // Generate cache key based on tenant and application identifier
    var cacheKey = $"AppConfig:{CurrentTenant.Id ?? Guid.Empty}:{applicationId}"; // cache temporarily disabled

    //// Try to get from cache
    //var cachedConfig = _cache.Get<EleoncoreApplicationConfigurationValueObject>(cacheKey);
    //if (cachedConfig != null)
    //{
    //  return cachedConfig;
    //}

    // Step 1: Get ABP base configuration (localization, auth, current user, features, etc.)
    //var abpConfig = await _abpApplicationConfigurationAppService.GetAsync(new ApplicationConfigurationRequestOptions
    //{
    //  IncludeLocalizationResources = false
    //});

    // Step 2: Build base config from appsettings
    var appConfig = await BuildBaseConfigFromAppsettingsAsync(applicationId);

    // Step 3: Check if enrichment is enabled
    var enableEnrichment = _configuration.GetValue<bool>("ApplicationConfiguration:EnableEnrichment", false);

    if (enableEnrichment)
    {
      // Step 4: Request enrichment from Eleoncore via event bus
      try
      {
        var apps = await GetApplicationsAsync();
        var app = apps.FirstOrDefault(x => IsRequestedApp(x.Path, applicationId));
        appConfig.ClientApplication = app;
        appConfig.Modules = app?.Modules;
      }
      catch (Exception ex)
      {
        _logger.Log.LogError(ex, "Error during configuration enrichment. Falling back to base config.");
      }
    }

    // Cache the final configuration
    var cacheTtl = _configuration.GetValue<int>("ApplicationConfiguration:CacheTtlMinutes", 10);
    _cache.Set(
        cacheKey,
        appConfig,
        TimeSpan.FromMinutes(cacheTtl));

    return appConfig;
  }

  private async Task<List<ClientApplicationEto>> GetApplicationsAsync()
  {
    var key = $"EnrichedApplications:{CurrentTenant.Id ?? Guid.Empty}";

    var item = _cache.Get<List<ClientApplicationEto>>(key);
    if (item != null)
    {
      return item;
    }

    var enrichmentRequest = new EnrichApplicationsConfigurationRequestMsg
    {
      TenantId = CurrentTenant.Id,
    };

    var timeout = _configuration.GetValue<int>("ApplicationConfiguration:EnrichmentTimeoutSeconds", 60);
    var response = await _eventBus.RequestAsync<EnrichedApplicationConfigurationResponseMsg>(
        enrichmentRequest, timeout);

    if (response?.IsSuccess == true && response.Applications != null && response.Applications.Count > 0)
    {
      _cache.Set(
          key,
          response.Applications,
          TimeSpan.FromMinutes(_configuration.GetValue<int>("ApplicationConfiguration:CacheTtlMinutes", 10))
      );
      return response.Applications;
    }

    return [];
  }

  private async Task<EleoncoreApplicationConfigurationValueObject> BuildBaseConfigFromAppsettingsAsync(
        string applicationIdentifier)
  {
    // Read applications from appsettings
    var appsSection = _configuration.GetSection("ApplicationConfiguration:Applications");
    var apps = appsSection.Get<List<AppsettingsApplicationDto>>() ?? new List<AppsettingsApplicationDto>();
    var app = apps.FirstOrDefault(a => IsRequestedApp(a.Path, applicationIdentifier));

    var response = await _eventBus.RequestAsync<GetBaseAppConfigResponseMsg>(new GetBaseAppConfigRequestMsg { UserId = _currentUser.Id });

    var baseConfig = response.ApplicationConfiguration;

    baseConfig.CurrentTenant = new Volo.Abp.AspNetCore.Mvc.MultiTenancy.CurrentTenantDto
    {
      Id = CurrentTenant.Id,
      Name = CurrentTenant.Name,
      IsAvailable = CurrentTenant.IsAvailable,
    };
    baseConfig.CurrentUser = new CurrentUserDto
    {
      IsAuthenticated = _currentUser.IsAuthenticated,
      Id = _currentUser.Id,
      TenantId = _currentUser.TenantId,
      ImpersonatorUserId = _currentUser.FindImpersonatorUserId(),
      ImpersonatorTenantId = _currentUser.FindImpersonatorTenantId(),
      ImpersonatorUserName = _currentUser.FindImpersonatorUserName(),
      ImpersonatorTenantName = _currentUser.FindImpersonatorTenantName(),
      UserName = _currentUser.UserName,
      Name = _currentUser.Name,
      SurName = _currentUser.SurName,
      Email = _currentUser.Email,
      EmailVerified = _currentUser.EmailVerified,
      PhoneNumber = _currentUser.PhoneNumber,
      PhoneNumberVerified = _currentUser.PhoneNumberVerified,
      Roles = _currentUser.Roles,
      SessionId = _currentUser.FindSessionId(),
    };

    // If application found in appsettings, populate application-specific fields
    if (app != null)
    {
      baseConfig.ApplicationName = app.Name;
      baseConfig.ApplicationPath = app.Path;
      baseConfig.FrameworkType = app.FrameworkType;
      baseConfig.StyleType = app.StyleType;
      baseConfig.ClientApplicationType = app.ClientApplicationType;
      baseConfig.ClientApplication = new ClientApplicationEto
      {
        AppType = ModuleCollector.Commons.Module.Proxy.Constants.ApplicationType.Application,
        ErrorHandlingLevel = ModuleCollector.Commons.Module.Proxy.Constants.ErrorHandlingLevel.Critical,
        Properties = new List<Messaging.Module.ETO.ApplicationPropertyEto>(),
        Name = app.Name,
        Path = app.Path,
        FrameworkType = Enum.TryParse<ClientApplicationFrameworkType>(app.FrameworkType, true, out var frameworkType) ? frameworkType : ClientApplicationFrameworkType.Angular,
        StyleType = Enum.TryParse<ClientApplicationStyleType>(app.StyleType, true, out var styleType) ? styleType : ClientApplicationStyleType.PrimeNg,
        ClientApplicationType = Enum.TryParse<ClientApplicationType>(app.ClientApplicationType, true, out var clientAppType) ? clientAppType : ClientApplicationType.Portal,
        IsEnabled = true,
        IsDefault = false,
        IsSystem = true,
        LoadLevel = nameof(UiModuleLoadLevel.Auto)
      };
      // Map modules from appsettings (if any)
      baseConfig.Modules = app.Modules?.Select(m => new Messaging.Module.ETO.ApplicationModuleEto
      {
        Name = $"{m.PluginName}_{m.Expose.Substring(2)}",
        Url = m.Url,
        PluginName = m.PluginName,
        OrderIndex = m.OrderIndex,
        Expose = m.Expose,
        LoadLevel = Enum.TryParse<UiModuleLoadLevel>(m.LoadLevel, true, out var lvl) ? lvl : UiModuleLoadLevel.Auto,
      }).ToList() ?? new List<Messaging.Module.ETO.ApplicationModuleEto>();
    }

    return baseConfig;
  }

  private static bool IsRequestedApp(string appId, string targetAppId)
  {
    appId = (appId?.Trim() ?? string.Empty).EnsureStartsWith('/').EnsureEndsWith('/');
    targetAppId = (targetAppId?.Trim() ?? string.Empty).EnsureStartsWith('/').EnsureEndsWith('/');
    return string.Equals(appId, targetAppId, StringComparison.OrdinalIgnoreCase);
  }

  // DTO for reading from appsettings
  private class AppsettingsApplicationDto
  {
    public string Name { get; set; }
    public string Path { get; set; }
    public string FrameworkType { get; set; }
    public string StyleType { get; set; }
    public string ClientApplicationType { get; set; }
    public List<AppsettingsModuleDto> Modules { get; set; }
  }

  private class AppsettingsModuleDto
  {
    public string Url { get; set; }
    public string PluginName { get; set; }
    public int OrderIndex { get; set; }
    public string Expose { get; set; }
    public string LoadLevel { get; set; }
  }
}
