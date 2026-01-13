using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Authorization.Module.TenantHostname;
using Common.Module.Extensions;
using Common.Module.Helpers;
using Logging.Module;
using MassTransit;
using MassTransit.Initializers;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Helpers.Module;
using Migrations.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions;
using Polly;
using TenantSettings.Module.Cache;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.TenantManagement.Module.ClientIsolation;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Repositories;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class ClientIsolationDomainService : DomainService
  {
    private readonly IVportalLogger<ClientIsolationDomainService> logger;
    private readonly IdentityUserManager identityUserManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICurrentUser currentUser;
    private readonly TenantSettingDomainService tenantSettingDomainService;
    private readonly IUserIsolationSettingsRepository userIsolationSettingsRepository;
    private readonly ClientIsolationValidator clientIsolationValidator;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly TenantSettingsCacheService tenantSettingsCacheService;
    private readonly IConfiguration configuration;
    private readonly ICurrentTenant currentTenant;

    public ClientIsolationDomainService(
        IVportalLogger<ClientIsolationDomainService> logger,
        IdentityUserManager identityUserManager,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUser currentUser,
        TenantSettingDomainService tenantSettingDomainService,
        IUserIsolationSettingsRepository userIsolationSettingsRepository,
        ClientIsolationValidator clientIsolationValidator,
        IDistributedEventBus massTransitPublisher,
        TenantSettingsCacheService tenantSettingsCacheService,
        IConfiguration configuration,
        ICurrentTenant currentTenant)
    {
      this.logger = logger;
      this.identityUserManager = identityUserManager;
      this.httpContextAccessor = httpContextAccessor;
      this.currentUser = currentUser;
      this.tenantSettingDomainService = tenantSettingDomainService;
      this.userIsolationSettingsRepository = userIsolationSettingsRepository;
      this.clientIsolationValidator = clientIsolationValidator;
      this.massTransitPublisher = massTransitPublisher;
      this.tenantSettingsCacheService = tenantSettingsCacheService;
      this.configuration = configuration;
      this.currentTenant = currentTenant;
    }

    public async Task SetTenantIsolationWithReplication(Guid? tenantId, bool enabled, byte[] clientCertificate, string certificatePassword)
    {
      try
      {
        await SetTenantIsolation(tenantId, enabled, clientCertificate, certificatePassword);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.Change(null))
        //    {
        //        await SetTenantIsolation(tenantId, enabled, clientCertificate, certificatePassword);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task SetTenantIsolation(Guid? tenantId, bool enabled, byte[] clientCertificate, string certificatePassword)
    {
      await EnsureAdmin();
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      settings.TenantIsolationEnabled = enabled;
      if (enabled)
      {
        var cert = CertificateHelper.CreateCertificateFromPem(clientCertificate, certificatePassword);
        settings.TenantCertificateHash = CertificateHelper.GetCertificateHash(cert);
      }
      else
      {
        settings.TenantCertificateHash = null;
      }

      await tenantSettingDomainService.UpdateSettings(settings);
    }

    public async Task SetUserIsolationWithReplication(Guid userId, bool enabled, byte[] clientCertificate, string certificatePassword)
    {
      try
      {
        await SetUserIsolation(userId, enabled, clientCertificate, certificatePassword);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.ChangeDefault())
        //    {
        //        await SetUserIsolation(userId, enabled, clientCertificate, certificatePassword);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
        throw new UserFriendlyException(ex.Message);
      }

    }

    private async Task SetUserIsolation(Guid userId, bool enabled, byte[] clientCertificate, string certificatePassword)
    {
      await EnsureAdmin(requireHost: false);

      if (string.IsNullOrWhiteSpace(certificatePassword))
      {
        throw new UserFriendlyException("PasswordIsInvalid");
      }

      var tenantSettings = await tenantSettingDomainService.GetOrCreateTenantSettings(currentTenant.Id);
      if (tenantSettings.TenantIsolationEnabled)
      {
        throw new Exception("Unable to set user isolation when tenant isolation is enabled.");
      }

      var settings = await userIsolationSettingsRepository.GetByUserIdAsync(userId);
      if (settings == null)
      {
        settings = new UserIsolationSettingsEntity(GuidGenerator.Create())
        {
          UserId = userId,
        };

        SetUserIsolationData(settings, enabled, clientCertificate, certificatePassword);
        await userIsolationSettingsRepository.InsertAsync(settings, true);
      }
      else
      {
        // Idempotency: check if already in desired state (simple check for enabled flag)
        // Note: certificate comparison is complex, so we only check the enabled flag
        if (settings.UserIsolationEnabled == enabled)
        {
          logger.Log.LogInformation(
            "User isolation settings for user {UserId} already have IsEnabled={Enabled}. Treating as idempotent success.",
            userId, enabled);
          await NotifySettingsChanged();
          return;
        }

        SetUserIsolationData(settings, enabled, clientCertificate, certificatePassword);
        try
        {
          await userIsolationSettingsRepository.UpdateAsync(settings, true);
        }
        catch (AbpDbConcurrencyException ex)
        {
          logger.Log.LogWarning(
            ex,
            "Concurrency conflict while updating user isolation settings for user {UserId}. Waiting for desired state...",
            userId);

          await ConcurrencyExtensions.WaitForDesiredStateAsync(
            async () =>
            {
              var currentSettings = await userIsolationSettingsRepository.GetByUserIdAsync(userId);
              var isDesired = currentSettings != null && currentSettings.UserIsolationEnabled == enabled;
              var details = currentSettings == null
                ? "Settings not found"
                : $"IsEnabled={currentSettings.UserIsolationEnabled}";
              return new ConcurrencyExtensions.ConcurrencyWaitResult<UserIsolationSettingsEntity>(isDesired, currentSettings, details);
            },
            logger.Log,
            "SetUserIsolation",
            userId
          );
        }
      }

      await NotifySettingsChanged();
    }

    public async Task<UserIsolationSettingsEntity> GetUserIsolationSettings(Guid userId)
    {
      UserIsolationSettingsEntity result = null;
      try
      {
        await EnsureAdmin(requireHost: false);
        var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(currentTenant.Id);
        result =
            await userIsolationSettingsRepository.GetByUserIdAsync(userId)
            ?? new UserIsolationSettingsEntity(Guid.Empty) { UserId = userId, UserIsolationEnabled = false };
        result.TenantIsolationEnabled = settings.TenantIsolationEnabled;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetTenantIpIsolationSettingsWithReplication(Guid? tenantId, bool ipIsolationEnabled, List<TenantWhitelistedIpEntity> whitelistedIps)
    {
      try
      {
        await SetTenantIpIsolationSettings(tenantId, ipIsolationEnabled, whitelistedIps);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.ChangeDefault())
        //    {
        //        await SetTenantIpIsolationSettings(tenantId, ipIsolationEnabled, whitelistedIps);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task SetTenantIpIsolationSettings(Guid? tenantId, bool ipIsolationEnabled, List<TenantWhitelistedIpEntity> whitelistedIps)
    {
      await EnsureAdmin(requireHost: true);
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      var changedIps = whitelistedIps
          .Select(x => new TenantWhitelistedIpEntity(GuidGenerator.Create())
          {
            TenantId = tenantId,
            IpAddress = x.IpAddress,
            Enabled = x.Enabled
          })
          .ToList();

      settings.WhitelistedIps = changedIps;
      settings.IpIsolationEnabled = ipIsolationEnabled;
      await tenantSettingDomainService.UpdateSettings(settings);
    }

    private void SetUserIsolationData(UserIsolationSettingsEntity settings, bool enabled, byte[] clientCertificate, string certificatePassword)
    {
      settings.UserIsolationEnabled = enabled;
      if (enabled)
      {
        var cert = CertificateHelper.CreateCertificateFromPem(clientCertificate, certificatePassword);
        settings.UserCertificateHash = CertificateHelper.GetCertificateHash(cert);
      }
      else
      {
        settings.UserCertificateHash = null;
      }
    }

    private async Task NotifySettingsChanged()
    {
      if (bool.TryParse(configuration["MassTransitOptions:Enabled"], out bool enabled) && enabled)
      {
        var message = new TenantSettingsUpdatedMsg();
        await massTransitPublisher.PublishAsync(message);
      }
    }

    private async Task EnsureAdmin(bool requireHost = true)
    {
      if (currentTenant.Id != null && requireHost)
      {
        throw new Exception("Only host can change this setting.");
      }

      var user = await identityUserManager.GetByIdAsync(currentUser.Id.Value);
      bool isAdmin = await identityUserManager.IsInRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue);
      if (!isAdmin)
      {
        throw new Exception("Only admin can change this setting.");
      }
    }

    public async Task Validate(string host, string remoteIpAddress, string forwardedFor, X509Certificate2? clientCertificate)
    {
      var validationResult = await clientIsolationValidator.ValidateClientIsolation(remoteIpAddress, forwardedFor, clientCertificate, currentUser.Id);

      bool isSecureRequest = await clientIsolationValidator.IsSecureHost(host, currentTenant.Id);
      //if (validationResult.ValidationResult
      //    is ClientIsolationValidationResult.CertIsolationDisabled
      //    && isSecureRequest)
      //{
      //    logger.Log.LogDebug($"Redirecting user {validationResult.Ip} to non-secure API.");
      //    await SendRedirectToNonSecure(validationResult, currentTenant.Id);
      //}
      //else if (validationResult.Valid)
      //{
      //    await next(context);
      //}
      //else if (validationResult.ValidationResult == ClientIsolationValidationResult.InvalidIp)
      //{
      //    logger.Log.LogDebug($"Rejecting user {validationResult.Ip} because of IP isolation validation failure.");
      //    SendIpError(context);
      //}
      //else if (isSecureRequest)
      //{
      //    logger.Log.LogDebug($"Redirect user {validationResult.Ip} to the error page as he failed to provide cert via secure API.");
      //    var currentUrl = await GetBaseTenantUrl(host, currentTenant.tenantId);
      //    string errorUrl = $"{currentUrl}/srv/core/certificate-error";
      //    SetCustomRedirect(context, errorUrl);
      //    await SendRedirectToCertificateError(context, currentTenant.Id);
      //}
      //else
      //{
      //    logger.Log.LogDebug($"Redirect user {validationResult.Ip} secure API.");
      //    await SendRedirectToSecure(context, currentTenant.Id);
      //}
    }

    private void SendIpError(HttpContext context)
    {
      context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }

    private async Task SendRedirectToCertificateError(HttpContext context, Guid? tenantId)
    {
    }

    private async Task SendRedirectToSecure(HttpContext context, Guid? tenantId)
    {
      //var secureBaseUrl = await tenantUrlResolver.GetBaseTenantUrl(context, tenantId, security: true);
      //SetCustomRedirect(context, secureBaseUrl);
    }

    private async Task SendRedirectToNonSecure(ValidationResult validationResult, Guid? tenantId)
    {
      //var nonSecureBaseUrl = await GetBaseTenantUrl(context, tenantId, security: false);
      //SetCustomRedirect(context, nonSecureBaseUrl);
    }


    private static string Https(string host) => $"https://{host}";
    public async ValueTask<string> GetBaseTenantUrl(string host, Guid? tenantId, bool? security = null)
    {
      if (security == null)
      {
        return Https(host);
      }

      var settings = await tenantSettingsCacheService.GetTenantSettings(tenantId);
      if (security == true)
      {
        if (settings.TenantSecureHostnames.Contains(host))
        {
          return Https(host);
        }

        return Https(settings.TenantSecureHostnames.First());
      }

      if (settings.TenantNonSecureHostnames.Contains(host))
      {
        return Https(host);
      }

      return Https(settings.TenantNonSecureHostnames.First());
    }

    private void SetCustomRedirect(HttpContext context, string targetUrl)
    {
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      context.Response.Headers["Location"] = targetUrl;
    }
  }
}
