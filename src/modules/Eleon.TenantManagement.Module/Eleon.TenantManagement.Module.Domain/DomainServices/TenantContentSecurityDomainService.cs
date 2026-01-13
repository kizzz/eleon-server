using Logging.Module;
using Microsoft.Extensions.Localization;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module.DomainServices
{
  public class TenantContentSecurityDomainService : DomainService
  {
    private readonly IVportalLogger<TenantContentSecurityDomainService> logger;
    private readonly IStringLocalizer<TenantManagementResource> localizer;
    private readonly TenantSettingDomainService tenantSettingDomainService;

    public TenantContentSecurityDomainService(
        IVportalLogger<TenantContentSecurityDomainService> logger,
        IStringLocalizer<TenantManagementResource> localizer,
        TenantSettingDomainService tenantSettingDomainService)
    {
      this.logger = logger;
      this.localizer = localizer;
      this.tenantSettingDomainService = tenantSettingDomainService;
    }

    public async Task AddTenantContentSecurityHostWithReplication(Guid? tenantId, string hostname)
    {
      try
      {
        await AddTenantContentSecurityHost(tenantId, hostname);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.ChangeDefault())
        //    {
        //        await AddTenantContentSecurityHost(tenantId, hostname);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task RemoveTenantContentSecurityHostWithReplication(Guid? tenantId, Guid securityHostId)
    {
      try
      {
        await RemoveTenantContentSecurityHost(tenantId, securityHostId);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.Change(Guid.Empty))
        //    {
        //        await RemoveTenantContentSecurityHost(tenantId, securityHostId);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task UpdateTenantContentSecurityHostWithReplication(Guid? tenantId, Guid securityHostId, string newHostname)
    {
      try
      {
        await UpdateTenantContentSecurityHost(tenantId, securityHostId, newHostname);

        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.Change(Guid.Empty))
        //    {
        //        await UpdateTenantContentSecurityHost(tenantId, securityHostId, newHostname);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task UpdateTenantContentSecurityHost(Guid? tenantId, Guid securityHostId, string newHostname)
    {
      string sanitizedHostname = SanitizeHostname(newHostname);
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);

      var host = settings.ContentSecurityHosts
          .FirstOrDefault(x => x.Id == securityHostId)
          ?? throw new UserFriendlyException(localizer["ContentSecurityHostDoesNotExist"]);

      EnsureHostnameUnique(settings, sanitizedHostname);

      host.Hostname = sanitizedHostname;
      await tenantSettingDomainService.UpdateSettings(settings);
    }

    private async Task RemoveTenantContentSecurityHost(Guid? tenantId, Guid securityHostId)
    {
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);

      var host = settings.ContentSecurityHosts
          .FirstOrDefault(x => x.Id == securityHostId)
          ?? throw new UserFriendlyException(localizer["ContentSecurityHostDoesNotExist"]);

      settings.ContentSecurityHosts.Remove(host);
      await tenantSettingDomainService.UpdateSettings(settings);
    }

    private void EnsureHostnameUnique(TenantSettingEntity settings, string hostname)
    {
      var existing = settings.ContentSecurityHosts
          .FirstOrDefault(x => x.Hostname.Equals(hostname, StringComparison.OrdinalIgnoreCase));

      if (existing != null)
      {
        throw new UserFriendlyException(localizer["ContentSecurityHostAlreadyExists", hostname]);
      }
    }

    private async Task AddTenantContentSecurityHost(Guid? tenantId, string hostname)
    {
      string sanitizedHostname = SanitizeHostname(hostname);
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      EnsureHostnameUnique(settings, sanitizedHostname);

      var host = new TenantContentSecurityHostEntity(GuidGenerator.Create(), sanitizedHostname);
      settings.ContentSecurityHosts.Add(host);
      await tenantSettingDomainService.UpdateSettings(settings);
    }

    private static string SanitizeHostname(string hostname) => hostname.Trim().Trim('/', '\\');
  }
}
