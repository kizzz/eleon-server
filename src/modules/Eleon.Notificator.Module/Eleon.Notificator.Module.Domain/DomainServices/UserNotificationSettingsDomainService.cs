using Common.Module.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.Repositories;
using VPortal.Notificator.Module.UserNotificationSettings;

namespace VPortal.Notificator.Module.DomainServices
{

  public class UserNotificationSettingsDomainService : DomainService
  {
    private readonly IVportalLogger<UserNotificationSettingsDomainService> logger;
    private readonly IUserNotificationSettingsRepository userNotificationSettingsRepository;

    public UserNotificationSettingsDomainService(
        IVportalLogger<UserNotificationSettingsDomainService> logger,
        IUserNotificationSettingsRepository userNotificationSettingsRepository)
    {
      this.logger = logger;
      this.userNotificationSettingsRepository = userNotificationSettingsRepository;
    }

    public async Task SetUserNotificationSettings(Guid userId, NotificationSourceType sourceType, bool sendNative, bool sendEmail)
    {
      try
      {
        var existing = await userNotificationSettingsRepository.GetSettings(userId, sourceType);
        if (existing == null)
        {
          var newSettings = new UserNotificationSettingsEntity(GuidGenerator.Create(), userId, sourceType, sendNative, sendEmail);
          await userNotificationSettingsRepository.InsertAsync(newSettings);
        }
        else
        {
          existing.SendEmail = sendEmail;
          existing.SendNative = sendNative;
          await userNotificationSettingsRepository.UpdateAsync(existing);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<UserNotificationSettingsEntity>> GetUserNotificationSettings(Guid userId)
    {
      List<UserNotificationSettingsEntity> result = null;
      try
      {
        var settings = new List<UserNotificationSettingsEntity>();
        foreach (var def in UserNotificationSettingsConsts.Defaults)
        {
          settings.Add(await GetSettingsOrDefault(userId, def.Key, def.Value));
        }

        result = settings;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    internal async Task<Dictionary<Guid, UserNotificationSettingsEntity>> GetUserNotificationSettings(List<Guid> userIds, NotificationSourceType sourceType)
    {
      Dictionary<Guid, UserNotificationSettingsEntity> result = null;
      try
      {
        var settings = await userNotificationSettingsRepository.GetSettings(userIds, sourceType);
        result = userIds.Select(x => GetSettingsOrDefault(x, settings)).ToDictionary(x => x.UserId, x => x);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;

      UserNotificationSettingsEntity GetSettingsOrDefault(Guid userId, List<UserNotificationSettingsEntity> settings)
      {
        var existing = settings.FirstOrDefault(x => x.UserId == userId);
        if (existing != null)
        {
          return existing;
        }

        return CreateFromDefault(userId, sourceType, UserNotificationSettingsConsts.Defaults.GetValueOrDefault(sourceType));
      }
    }

    private async Task<UserNotificationSettingsEntity> GetSettingsOrDefault(Guid userId, NotificationSourceType sourceType, NotificationSourceTypeDefaults defaults)
    {
      var existing = await userNotificationSettingsRepository.GetSettings(userId, sourceType);
      if (existing != null)
      {
        return existing;
      }

      return CreateFromDefault(userId, sourceType, defaults);
    }

    private UserNotificationSettingsEntity CreateFromDefault(Guid userId, NotificationSourceType sourceType, NotificationSourceTypeDefaults defaults)
        => new UserNotificationSettingsEntity(Guid.Empty, userId, sourceType, defaults.SendNative, defaults.SendEmail);
  }
}
