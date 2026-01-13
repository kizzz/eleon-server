using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSettings.Module.Helpers;
using TenantSettings.Module.Messaging;
using TenantSettings.Module.Models;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Repositories;

namespace VPortal.Identity.Module.EventServices
{
  public class GetTenantSettingsEventService :
      IDistributedEventHandler<GetTenantSettingsMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetTenantSettingsEventService> logger;
    private readonly TenantSettingDomainService tenantSettingDomainService;
    private readonly IUserIsolationSettingsRepository userIsolationSettingsRepository;
    private readonly IdentityUserManager identityUserManager;
    private readonly IObjectMapper mapper;
    private readonly IServiceProvider serviceProvider;
    private readonly ICurrentTenant currentTenant;
    private readonly IResponseContext responseContext;
    private readonly IConfiguration configuration;

    private static readonly SemaphoreSlim tenantSettingsAccess = new(1, 1);
    private static DateTime LastAccessTime = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private static string CachedSettingsJson = null;

    public GetTenantSettingsEventService(
        IVportalLogger<GetTenantSettingsEventService> logger,
        TenantSettingDomainService tenantSettingDomainService,
        IUserIsolationSettingsRepository userIsolationSettingsRepository,
        IdentityUserManager identityUserManager,
        IObjectMapper mapper,
        IServiceProvider serviceProvider,
        ICurrentTenant currentTenant,
        IResponseContext responseContext,
        IConfiguration configuration)
    {
      this.logger = logger;
      this.tenantSettingDomainService = tenantSettingDomainService;
      this.userIsolationSettingsRepository = userIsolationSettingsRepository;
      this.identityUserManager = identityUserManager;
      this.mapper = mapper;
      this.serviceProvider = serviceProvider;
      this.currentTenant = currentTenant;
      this.responseContext = responseContext;
      this.configuration = configuration;
    }

    public async Task HandleEventAsync(GetTenantSettingsMsg eventData)
    {
      var response = new TenantSettingsGotMsg();
      try
      {
        await tenantSettingsAccess.WaitAsync();

        try
        {
          if (!string.IsNullOrEmpty(CachedSettingsJson) && (DateTime.UtcNow - LastAccessTime) < CacheDuration)
          {
            response.SettingsJson = CachedSettingsJson;
            return;
          }

          using (currentTenant.Change(null))
          {
            var settings = await tenantSettingDomainService.GetAllSettings();

            var multitenancyService = serviceProvider.GetRequiredService<MultiTenancyDomainService>();
            var userSettings = await multitenancyService.CollectForAllTenants(async _ => await userIsolationSettingsRepository.GetListAsync());

            var admins = await identityUserManager.GetUsersInRoleAsync(MigrationConsts.AdminRoleNameDefaultValue);
            var adminIds = admins.Select(x => x.Id).ToList();

            var corsConfig = configuration.GetSection("Cors");
            string[] cors = Array.Empty<string>();
            if (corsConfig.Exists())
            {
              cors = corsConfig.Get<string[]>();
            }

            var cacheEto = new TenantSettingsCacheEto()
            {
              TenantSettings = mapper.Map<List<TenantSettingEntity>, List<TenantSetting>>(settings),
              UserIsolationSettings = mapper.Map<List<UserIsolationSettingsEntity>, List<UserIsolationSettings>>(userSettings),
              Cors = cors,
              HostAdminUsers = adminIds,
            };

            LastAccessTime = DateTime.UtcNow;
            CachedSettingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(cacheEto);
            response.SettingsJson = CachedSettingsJson;
          }
        }
        finally
        {
          tenantSettingsAccess.Release();
        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
