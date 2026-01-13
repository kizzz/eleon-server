using Core.Infrastructure.Module.Entities;
using Core.Infrastructure.Module.Repositories;
using Logging.Module;
using Migrations.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Core.Infrastructure.Module.DomainServices
{

  public class DashboardSettingDomainService : DomainService
  {
    private readonly IVportalLogger<DashboardSettingDomainService> logger;
    private readonly IDashboardSettingRepository dashboardSettingsRepository;
    private readonly ICurrentUser currentUser;
    private readonly IdentityUserManager userManager;

    public DashboardSettingDomainService(
        IVportalLogger<DashboardSettingDomainService> logger,
        IDashboardSettingRepository dashboardSettingsRepository,
        ICurrentUser currentUser,
        IdentityUserManager userManager)
    {
      this.logger = logger;
      this.dashboardSettingsRepository = dashboardSettingsRepository;
      this.currentUser = currentUser;
      this.userManager = userManager;
    }

    public async Task<List<DashboardSettingEntity>> GetDashboardSettings()
    {
      List<DashboardSettingEntity> result = new List<DashboardSettingEntity>();
      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new AbpAuthorizationException();
        }

        result = await dashboardSettingsRepository.GetList(currentUser.Id.Value);

        if (result.Count == 0)
        {
          result = await GetDefaultDashboardSettings();
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

    public async Task<List<DashboardSettingEntity>> GetDefaultDashboardSettings()
    {
      List<DashboardSettingEntity> result = new List<DashboardSettingEntity>();
      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new AbpAuthorizationException();
        }

        result = await dashboardSettingsRepository.GetDefaultDashboardSettings();
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

    public async Task<string> CreateOrUpdateSettings(List<DashboardSettingEntity> dashboardSettingEntities, bool setAsDefault)
    {
      string result = string.Empty;
      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new AbpAuthorizationException();
        }

        List<Guid> idsToRemove = new List<Guid>();
        var allSettings = await dashboardSettingsRepository.GetListAsync();
        if (allSettings != null && allSettings.Count > 0)
        {
          idsToRemove = allSettings
              .Where(dbSetting => !dashboardSettingEntities.Any(uiSetting => uiSetting.Id == dbSetting.Id))
              .Select(dbSetting => dbSetting.Id)
              .ToList();

          if (idsToRemove.Count > 0)
          {
            await dashboardSettingsRepository.DeleteManyAsync(idsToRemove, true);
          }
        }

        if (setAsDefault)
        {
          await IsAdmin();
        }

        foreach (var dashboardSettingEntity in dashboardSettingEntities)
        {
          var entity = await dashboardSettingsRepository.FindAsync(dashboardSettingEntity.Id);
          if (entity != null)
          {
            entity.Label = dashboardSettingEntity.Label;
            entity.Template = dashboardSettingEntity.Template;
            entity.Cols = dashboardSettingEntity.Cols;
            entity.Rows = dashboardSettingEntity.Rows;
            entity.XCoordinate = dashboardSettingEntity.XCoordinate;
            entity.YCoordinate = dashboardSettingEntity.YCoordinate;
            entity.MaxItemCols = dashboardSettingEntity.MaxItemCols;
            entity.MinItemCols = dashboardSettingEntity.MinItemCols;
            entity.MaxItemRows = dashboardSettingEntity.MaxItemRows;
            entity.MinItemRows = dashboardSettingEntity.MinItemRows;
            entity.IsDefault = setAsDefault;
            await dashboardSettingsRepository.UpdateAsync(entity, true);
          }
          else
          {
            dashboardSettingEntity.DragEnabled = true;
            dashboardSettingEntity.ResizeEnabled = true;
            dashboardSettingEntity.CompactEnabled = false;
            dashboardSettingEntity.UserId = currentUser.Id.Value;
            dashboardSettingEntity.TenantId = CurrentTenant.Id;
            dashboardSettingEntity.IsDefault = setAsDefault;
            await dashboardSettingsRepository.InsertAsync(dashboardSettingEntity);
          }
        }
      }
      catch (Exception ex)
      {
        result = ex.Message;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<string> DeleteDashboardSettings(Guid dashboardSettingEntityId)
    {
      string result = string.Empty;
      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new AbpAuthorizationException();
        }

        var entity = await dashboardSettingsRepository.GetAsync(dashboardSettingEntityId);
        await dashboardSettingsRepository.DeleteAsync(entity, true);
      }
      catch (Exception ex)
      {
        result = ex.Message;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

      return result;
    }

    private async Task IsAdmin()
    {
      var user = await userManager.GetByIdAsync(currentUser.Id.Value);
      bool isAdmin = await userManager.IsInRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue);
      if (!isAdmin)
      {
        throw new Exception("Only admin can change this setting.");
      }
    }
  }
}
