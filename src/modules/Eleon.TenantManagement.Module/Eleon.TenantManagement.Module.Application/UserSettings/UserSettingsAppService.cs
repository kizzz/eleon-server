using Common.Module.Helpers;
using Common.Module.ValueObjects;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using VPortal.Infrastructure.Module.Result;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.UserSettings;
using VPortal.TenantManagement.Module.ValueObjects;

namespace Core.Infrastructure.Module.UserSettings
{
  [Authorize]
  public class UserSettingsAppService : TenantManagementAppService, IUserSettingsAppService
  {
    private readonly IVportalLogger<UserSettingsAppService> logger;
    private readonly UserSettingDomainService userSettingDomainService;

    public UserSettingsAppService(
        IVportalLogger<UserSettingsAppService> logger,
        UserSettingDomainService userSettingDomainService
    )
    {
      this.logger = logger;
      this.userSettingDomainService = userSettingDomainService;
    }

    public async Task<ResultDto<string>> GetAppearanceSetting(string appId)
    {
      ResultDto<string> result = null;
      try
      {
        var entity = await userSettingDomainService.GetAppearanceSetting(appId);
        result = ObjectMapper.Map<ResultValueObject<string>, ResultDto<string>>(entity);

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

    public async Task<ResultDto<string>> SetAppearanceSetting(string setting, string appId)
    {
      ResultDto<string> result = null;
      try
      {
        var entity = await userSettingDomainService.SetAppearance(setting, appId);
        result = ObjectMapper.Map<ResultValueObject<string>, ResultDto<string>>(entity);
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

    public async Task<UserSettingDto> SetUserSettings(UserSettingDto userSettingDto)
    {
      UserSettingDto result = new();
      try
      {
        var mappedEntity = ObjectMapper.Map<UserSettingDto, UserSettingEntity>(userSettingDto);
        UserSettingEntity entity = await userSettingDomainService.SetUserSetting(mappedEntity);
        result = ObjectMapper.Map<UserSettingEntity, UserSettingDto>(entity);
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


    public async Task<UserSettingDto> GetUserSettingByUserId(Guid userId)
    {
      UserSettingDto result = new();
      try
      {
        UserSettingEntity entity = await userSettingDomainService.GetUserSettingByUserId(userId);

        result = ObjectMapper.Map<UserSettingEntity, UserSettingDto>(entity);
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

    public async Task<string> GetCurrentUserSettingAsync(string name)
    {
      try
      {
        if (CurrentUser.Id == null)
        {
          throw new AbpAuthorizationException();
        }

        return await userSettingDomainService.GetUserSettingAsync(CurrentUser.Id.Value, name);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return string.Empty;
    }

    public async Task SetCurrentUserSettingAsync(string name, string value)
    {
      try
      {
        if (CurrentUser.Id == null)
        {
          throw new AbpAuthorizationException();
        }

        await userSettingDomainService.SetUserSettingAsync(CurrentUser.Id.Value, name, value);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task<string> GetUserSettingAsync(Guid userId, string name)
    {
      try
      {
        return await userSettingDomainService.GetUserSettingAsync(userId, name);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return string.Empty;
    }

    public async Task SetUserSettingAsync(Guid userId, string name, string value)
    {
      try
      {
        await userSettingDomainService.SetUserSettingAsync(userId, name, value);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }
  }
}
