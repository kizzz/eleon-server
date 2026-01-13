using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Microsoft.Extensions.Logging;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using SharedModule.modules.Helpers.Module;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Migrations.Module;
using System.Reflection;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Users;
using VPortal.DocMessageLog.Module.Entities;
using VPortal.DocMessageLog.Module.Options;
using VPortal.DocMessageLog.Module.Repositories;
using VPortal.DocMessageLog.Module.Settings;

namespace VPortal.DocMessageLog.Module.Domain;


public class SystemLogDomainService : DomainService
{
  private readonly IVportalLogger<SystemLogDomainService> _logger;
  private readonly ISystemLogRepository _repository;
  private readonly IDistributedEventBus _distributedEventBus;
  private readonly IdentityUserManager _userManager;
  private readonly ISettingManager _settingManager;
  private readonly ISystemLogHubContext systemLogHubContext;

  public SystemLogDomainService(
      IVportalLogger<SystemLogDomainService> logger,
      ISystemLogRepository repository,
      IDistributedEventBus distributedEventBus,
      ISettingManager settingManager,
      IdentityUserManager userManager,
      ISystemLogHubContext systemLogHubContext)
  {
    _logger = logger;
    _repository = repository;
    _distributedEventBus = distributedEventBus;
    _settingManager = settingManager;
    _userManager = userManager;
    this.systemLogHubContext = systemLogHubContext;
  }

  public async Task<SystemLogEntity> GetByIdAsync(Guid id)
  {
    try
    {
      var result = await _repository.GetAsync(id);
      return result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<KeyValuePair<int, List<SystemLogEntity>>> GetListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string searchQuery = null,
      SystemLogLevel? minLogLevelFilter = SystemLogLevel.Info,
      string initiatorFilter = null,
      string initiatorTypeFilter = null,
      string applicationNameFilter = null,
      DateTime? creationFromDateFilter = null,
      DateTime? creationToDateFilter = null,
      bool onlyUnread = false)
  {
    KeyValuePair<int, List<SystemLogEntity>> result = default;
    try
    {
      result = await _repository.GetListAsync(
          sorting,
          maxResultCount,
          skipCount,
          searchQuery,
          minLogLevelFilter,
          initiatorFilter,
          initiatorTypeFilter,
          applicationNameFilter,
          creationFromDateFilter,
          creationToDateFilter,
          onlyUnread);

      var lastLog = result.Value.OrderByDescending(x => x.LastModificationTime).FirstOrDefault();
      if (lastLog != null)
      {
        var state = await _settingManager.GetOrDefaultForCurrentUserAsync<SystemLogUserStateOptions>(ModuleSettings.LastReadedLogState);

        if (!state.LastAckNotificationLog.HasValue || state.LastAckNotificationLog != lastLog.Id || state.LastAckDate < lastLog.LastModificationTime)
        {
          var admins = await _userManager.GetUsersInRoleAsync(MigrationConsts.AdminRoleNameDefaultValue);

          var adminIds = admins.Select(a => a.Id).ToList();

          await systemLogHubContext.PushSystemLogAsync(
              adminIds,
              new SystemLogEntity());

          state.Acknowledge(lastLog);
          await _settingManager.SetForCurrentUserAsync(ModuleSettings.LastReadedLogState, state);
        }
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }

    return result;
  }

  public async Task<int> WriteAsync(SystemLogEntity msgLog)
  {
    try
    {
      var logId = GuidGenerator.Create();
      var toAdd = new SystemLogEntity(logId)
      {
        Message = msgLog.Message,
        LogLevel = msgLog.LogLevel,
        ApplicationName = msgLog.ApplicationName,
        InitiatorId = msgLog.InitiatorId,
        InitiatorType = msgLog.InitiatorType,
        TenantId = msgLog.TenantId
      };

      foreach (var prop in msgLog.ExtraProperties)
      {
        toAdd.SetProperty(prop.Key, prop.Value?.ToString());
      }

      var result = await _repository.WriteAsync(toAdd);

      if (
          result.Count <= 1 ||
          (result.ExtraProperties.GetOrDefault("Time")?.ToString() == toAdd.ExtraProperties.GetOrDefault("Time")?.ToString() && (!string.IsNullOrEmpty(result.ExtraProperties.GetOrDefault("Time")?.ToString()))) // to send notification for logs that was written with db sink firstly
          )
      {
        var admins = await _userManager.GetUsersInRoleAsync(MigrationConsts.AdminRoleNameDefaultValue);

        var adminIds = admins.Select(a => a.Id).ToList();

        await systemLogHubContext.PushSystemLogAsync(
            adminIds,
            new SystemLogEntity());

        await _distributedEventBus.PublishAsync(new AddNotificationsMsg
        {
          Notifications = new List<EleonsoftNotification>
                    {
                        new EleonsoftNotification
                        {
                            Message = toAdd.Message,
                            Type = new SystemNotificationType
                            {
                                ExtraProperties = toAdd.ExtraProperties.ToDictionary(x => x.Key, x => x.Value?.ToString()),
                                LogLevel = toAdd.LogLevel,
                                WriteLog = false,
                            },
                            RunImmidiate = true,

                            Priority = toAdd.LogLevel == SystemLogLevel.Critical ? NotificationPriority.High : NotificationPriority.Normal,
                        }
                    }
        });
      }

      return result.Count;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<bool> WriteManyAsync(List<SystemLogEntity> msgLogs)
  {
    try
    {
      var notifications = new List<EleonsoftNotification>();
      foreach (var msgLog in msgLogs)
      {
        var logId = GuidGenerator.Create();
        var toAdd = new SystemLogEntity(logId)
        {
          Message = msgLog.Message,
          LogLevel = msgLog.LogLevel,
          ApplicationName = msgLog.ApplicationName,
          InitiatorId = msgLog.InitiatorId,
          InitiatorType = msgLog.InitiatorType,
          TenantId = msgLog.TenantId
        };
        foreach (var prop in msgLog.ExtraProperties)
        {
          toAdd.SetProperty(prop.Key, prop.Value?.ToString());
        }

        var result = await _repository.WriteAsync(toAdd);

        if (result.Count <= 1)
        {
          notifications.Add(
              new EleonsoftNotification
              {
                Message = toAdd.Message,
                Type = new SystemNotificationType
                {
                  ExtraProperties = toAdd.ExtraProperties.ToDictionary(x => x.Key, x => x.Value?.ToString()),
                  LogLevel = toAdd.LogLevel,
                  WriteLog = false,
                },
                RunImmidiate = true,

                Priority = toAdd.LogLevel == SystemLogLevel.Critical ? NotificationPriority.High : NotificationPriority.Normal,
              });
        }
      }

      if (notifications.Count > 0)
      {
        var admins = await _userManager.GetUsersInRoleAsync(MigrationConsts.AdminRoleNameDefaultValue);

        var adminIds = admins.Select(a => a.Id).ToList();

        await systemLogHubContext.PushSystemLogAsync(
            adminIds,
            new SystemLogEntity());
        await _distributedEventBus.PublishAsync(new AddNotificationsMsg { Notifications = notifications });
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }
    return true;
  }

  public async Task<bool> UpdateLogAsync(SystemLogEntity msgLog)
  {
    try
    {
      var existingEntity = await _repository.GetAsync(msgLog.Id);

      // Idempotency: check if already in desired state
      if (existingEntity.Message == msgLog.Message && existingEntity.LogLevel == msgLog.LogLevel)
      {
        _logger.Log.LogInformation(
          "System log {LogId} already has Message and LogLevel matching desired values. Treating as idempotent success.",
          msgLog.Id);
        return true;
      }

      existingEntity.Message = msgLog.Message;
      existingEntity.LogLevel = msgLog.LogLevel;

      try
      {
        var updatedEntity = await _repository.UpdateAsync(existingEntity, true);
      }
      catch (AbpDbConcurrencyException ex)
      {
        _logger.Log.LogWarning(
          ex,
          "Concurrency conflict while updating system log {LogId}. Waiting for desired state...",
          msgLog.Id);

        await ConcurrencyExtensions.WaitForDesiredStateAsync(
          async () =>
          {
            var currentEntity = await _repository.GetAsync(msgLog.Id);
            var isDesired = currentEntity.Message == msgLog.Message && currentEntity.LogLevel == msgLog.LogLevel;
            var details = $"Message={currentEntity.Message},LogLevel={currentEntity.LogLevel}";
            return new ConcurrencyExtensions.ConcurrencyWaitResult<SystemLogEntity>(isDesired, currentEntity, details);
          },
          _logger.Log,
          "UpdateSystemLog",
          msgLog.Id
        );
      }
    }
    catch (AbpDbConcurrencyException)
    {
      throw; // Re-throw after handling above
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }

    return true;
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    try
    {
      await _repository.DeleteAsync(id);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }

    return true;
  }
  public async Task MarkAllReadedAsync()
  {
    try
    {
      await _repository.MarkAllReadedAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task MarkReadedAsync(Guid logId, bool isReaded)
  {
    try
    {
      var log = await _repository.GetAsync(logId);

      // Idempotency: check if already in desired state
      if (log.IsArchived == isReaded)
      {
        _logger.Log.LogInformation(
          "System log {LogId} already has IsArchived={IsArchived}. Treating as idempotent success.",
          logId, isReaded);
        return;
      }

      log.IsArchived = isReaded;
      try
      {
        await _repository.UpdateAsync(log, true);
      }
      catch (AbpDbConcurrencyException ex)
      {
        _logger.Log.LogWarning(
          ex,
          "Concurrency conflict while marking system log {LogId} as IsArchived={IsArchived}. Waiting for desired state...",
          logId, isReaded);

        await ConcurrencyExtensions.WaitForDesiredStateAsync(
          async () =>
          {
            var currentLog = await _repository.GetAsync(logId);
            var isDesired = currentLog.IsArchived == isReaded;
            var details = $"IsArchived={currentLog.IsArchived}";
            return new ConcurrencyExtensions.ConcurrencyWaitResult<SystemLogEntity>(isDesired, currentLog, details);
          },
          _logger.Log,
          "MarkSystemLogReaded",
          logId
        );
      }
    }
    catch (AbpDbConcurrencyException)
    {
      throw; // Re-throw after handling above
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<UnresolvedSystemLogCount> GetTotalUnresolvedCount()
  {
    try
    {
      var state = await _settingManager.GetOrDefaultForCurrentUserAsync<SystemLogUserStateOptions>(ModuleSettings.LastReadedLogState);
      return await _repository.GetTotalUnresolvedCountAsync(state.LastAckDate);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }
}
