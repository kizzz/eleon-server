using Common.Module.Constants;
using Logging.Module;
using MassTransit.Transports;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.Identity;
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
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Repositories;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices
{
  public class EleonsoftApplicationConfigurationDomainService : DomainService
  {
    private readonly IVportalLogger<EleonsoftApplicationConfigurationDomainService> logger;
    private readonly AbpApplicationConfigurationAppService abpApplicationConfigurationAppService;
    private readonly IConfiguration configuration;
    private readonly TenantSettingDomainService _tenantSettingDomainService;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IUserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser> _claimsPrincipalFactory;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    public EleonsoftApplicationConfigurationDomainService(
            IVportalLogger<EleonsoftApplicationConfigurationDomainService> logger,
            AbpApplicationConfigurationAppService abpApplicationConfigurationAppService,
            IConfiguration configuration,
            TenantSettingDomainService tenantSettingDomainService,
            IdentityUserManager identityUserManager,
            IUserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser> abpClaimsPrincipalFactory,
            ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
      this.logger = logger;
      this.abpApplicationConfigurationAppService = abpApplicationConfigurationAppService;
      this.configuration = configuration;
      _tenantSettingDomainService = tenantSettingDomainService;
      _identityUserManager = identityUserManager;
      _claimsPrincipalFactory = abpClaimsPrincipalFactory;
      _currentPrincipalAccessor = currentPrincipalAccessor;
    }

    public async Task<EleoncoreApplicationConfigurationValueObject> GetBaseAsync(Guid? userId)
    {
      EleoncoreApplicationConfigurationValueObject result = null;
      try
      {
        var user = userId.HasValue ? await _identityUserManager.FindByIdAsync(userId.Value.ToString()) : null;
        if (user != null)
        {
          var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
          using (_currentPrincipalAccessor.Change(claimsPrincipal))
          {
            result = await GetConfigAsync();
          }
        }
        else
        {
          result = await GetConfigAsync();
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

    private async Task<EleoncoreApplicationConfigurationValueObject> GetConfigAsync()
    {
      var authPath = configuration["App:AuthPath"];
      var corePath = configuration["App:CorePath"];
      var webPush = configuration["App:WebPush"];

      var applicationConfigurationDto = await abpApplicationConfigurationAppService.GetAsync(new ApplicationConfigurationRequestOptions() { IncludeLocalizationResources = false });

      var systemHealthSettings = await _tenantSettingDomainService.GetTenantSystemHealthSettings();
      applicationConfigurationDto.SetProperty("Telemetry", systemHealthSettings.Telemetry);
      applicationConfigurationDto.SetProperty("Logging", systemHealthSettings.Logging);

      if (applicationConfigurationDto.Localization.CurrentCulture.CultureName == "en-US")
      {
        applicationConfigurationDto.Localization.CurrentCulture.CultureName = "en";
        applicationConfigurationDto.Localization.CurrentCulture.TwoLetterIsoLanguageName = "en";
        applicationConfigurationDto.Localization.CurrentCulture.ThreeLetterIsoLanguageName = "eng";
        applicationConfigurationDto.Localization.CurrentCulture.Name = "en";
        applicationConfigurationDto.Localization.CurrentCulture.DisplayName = "English";
      }

      var result = new EleoncoreApplicationConfigurationValueObject
      {
        Localization = applicationConfigurationDto.Localization,
        Auth = applicationConfigurationDto.Auth,
        CurrentUser = applicationConfigurationDto.CurrentUser,
        Features = applicationConfigurationDto.Features,
        CurrentTenant = applicationConfigurationDto.CurrentTenant,
        ExtraProperties = applicationConfigurationDto.ExtraProperties,
        Production = false,
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
        },
        ApplicationName = null,
        ApplicationPath = null,
      };

      return result;
    }
  }
}
