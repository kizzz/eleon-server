using Common.Module.Constants;
using Logging.Module;
using Volo.Abp.Domain.Services;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.DomainServices
{
  public class TenantStatusDomainService : DomainService
  {
    private readonly IVportalLogger<TenantStatusDomainService> logger;
    private readonly TenantSettingDomainService tenantSettingDomainService;

    public TenantStatusDomainService(
        IVportalLogger<TenantStatusDomainService> logger,
        TenantSettingDomainService tenantSettingDomainService)
    {
      this.logger = logger;
      this.tenantSettingDomainService = tenantSettingDomainService;
    }

    public async Task SuspendTenantWithReplication(Guid tenantId)
    {
      try
      {
        await SuspendTenant(tenantId);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.Change(Guid.Empty))
        //    {
        //        await SuspendTenant(tenantId);
        //    }
        //}

      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task SuspendTenant(Guid tenantId)
    {
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      EnsureStatus(settings, TenantStatus.Active);
      settings.Status = TenantStatus.Suspended;
      await tenantSettingDomainService.UpdateSettings(settings);
    }

    public async Task ActivateTenantWithReplication(Guid tenantId)
    {
      try
      {
        await ActivateTenant(tenantId);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.Change(Guid.Empty))
        //    {
        //        await ActivateTenant(tenantId);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task ActivateTenant(Guid tenantId)
    {
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      //EnsureStatus(settings, TenantStatus.Suspended);
      settings.Status = TenantStatus.Active;
      await tenantSettingDomainService.UpdateSettings(settings);
    }

    public async Task CancelTenantWithReplication(Guid tenantId)
    {
      try
      {
        await CancelTenant(tenantId);
        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.Change(Guid.Empty))
        //    {
        //        await CancelTenant(tenantId);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task CancelTenant(Guid tenantId)
    {
      var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      //EnsureStatus(settings, TenantStatus.Active);
      settings.Status = TenantStatus.Terminated;
      await tenantSettingDomainService.UpdateSettings(settings);
    }

    private void EnsureStatus(TenantSettingEntity settings, TenantStatus status)
    {
      if (settings.Status != status)
      {
        throw new Exception($"This action is only allowed when tenant is {status}");
      }
    }
  }
}
