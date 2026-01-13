using Common.Module.Extensions;
using EleonsoftModuleCollector.Commons.Module.Constants.BackgroundJobs;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using ModuleCollector.Commons.Module.Constants;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplating;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.Repositories;

namespace VPortal.Notificator.Module.DomainServices
{
  public class NotificatorDomainService : DomainService
  {
    private readonly IVportalLogger<NotificatorDomainService> logger;
    private readonly INotificationsRepository notificationsRepository;
    private readonly ISettingManager settingManager;
    private readonly IDistributedEventBus _eventBus;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly IJsonSerializer jsonSerializer;
    private readonly IEmailSender emailSender;
    private readonly ICurrentUser currentUser;
    private readonly ICurrentTenant currentTenant;
    private readonly IConfiguration configuration;
    private readonly ITemplateRenderer templateRenderer;
    private readonly NotificatorHelperService _recepientResolver;
    private readonly IServiceProvider _serviceProvider;
    private readonly IdentityUserManager _userManager;
    private readonly INotificationHandler _notificationHandler;

    public NotificatorDomainService(IVportalLogger<NotificatorDomainService> logger,
        INotificationsRepository notificationsRepository,
        ISettingManager settingManager,
        IDistributedEventBus massTransitPublisher,
        IUnitOfWorkManager unitOfWorkManager,
        IJsonSerializer jsonSerializer,
        IEmailSender emailSender,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IConfiguration configuration,
        ITemplateRenderer templateRenderer,
        NotificatorHelperService recepientResolver,
        IServiceProvider serviceProvider,
        IdentityUserManager userManager,
        INotificationHandler notificationHandler)
    {
      this.logger = logger;
      this.notificationsRepository = notificationsRepository;
      this.settingManager = settingManager;
      this._eventBus = massTransitPublisher;
      this.unitOfWorkManager = unitOfWorkManager;
      this.jsonSerializer = jsonSerializer;
      this.emailSender = emailSender;
      this.currentUser = currentUser;
      this.currentTenant = currentTenant;
      this.configuration = configuration;
      this.templateRenderer = templateRenderer;
      _recepientResolver = recepientResolver;
      _serviceProvider = serviceProvider;
      _userManager = userManager;
      _notificationHandler = notificationHandler;
    }

    #region NotifyAsync

    public async Task<bool> NotifyAsync(EleonsoftNotification notification)
    {
      try
      {
        using var uow = unitOfWorkManager.Begin(true);

        var filters = configuration.GetSection("DebugSettings:Filters").Get<string[]>();

        if (filters != null && !filters.Contains("*", StringComparer.OrdinalIgnoreCase) && !filters.Contains(notification.Type.Type, StringComparer.OrdinalIgnoreCase))
        {
          return true;
        }

        await _notificationHandler.HandleAsync(notification);
        await uow.SaveChangesAsync();
        await uow.CompleteAsync();

        return true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    #endregion

    #region RequestBulkActiveNotificationsRunAsync

    public async Task RequestBulkActiveNotificationsRunAsync()
    {
      try
      {
        var activeNotifications = FilterNotifications(await notificationsRepository.GetActiveListAsync());
        if (activeNotifications.Count == 0)
        {
          return;
        }

        Guid backgroundJobId = GuidGenerator.Create();

        await MarkAsRunning(backgroundJobId, activeNotifications);

        var notificationEtos = MapEntitiesToEtos(activeNotifications);
        string initiatorDocuments = activeNotifications.JoinAsString(",");
        await CreateNotificationJobAsync(backgroundJobId, notificationEtos, initiatorDocuments);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    private async Task MarkAsRunning(Guid jobId, List<NotificationEntity> entities)
    {
      foreach (var notification in entities)
      {
        using (var unitOfWork = unitOfWorkManager.Begin())
        {
          notification.IsActive = false;
          notification.BackgroundJobId = jobId;
          await notificationsRepository.UpdateAsync(notification, true);
          await unitOfWork.CompleteAsync();
        }
        var message = new NotificatorExecutedMsg();
        await _eventBus.PublishAsync(message);
      }
    }

    private IEnumerable<EleonsoftNotification> MapEntitiesToEtos(IEnumerable<NotificationEntity> entities)
        => entities.Select(notification => new EleonsoftNotification()
        {
          Id = notification.Id,
          Recipients = jsonSerializer.Deserialize<List<RecipientEto>>(notification.Receivers),
          Message = notification.Message,
          //DataParams = notification.DataParams,
          Type = notification.Type,
          Priority = notification.Priority,
        });

    private async Task CreateNotificationJobAsync(Guid jobId, IEnumerable<EleonsoftNotification> notificationEtos, string initiatorDocument)
    {
      var jobParams = jsonSerializer.Serialize(notificationEtos);

      var jobMessage = new CreateBackgroundJobMsg
      {
        Id = jobId,
        Type = NotificatorBackgroundJobTypes.SendBulkNotification,
        ScheduleExecutionDateUtc = DateTime.UtcNow,
        IsRetryAllowed = false,
        StartExecutionParams = jobParams,
        StartExecutionExtraParams = initiatorDocument,
        SourceType = BackgroundJobConstants.SourceType.SystemModule,
        SourceId = "Notificator",
        TenantId = currentTenant.Id,
        TenantName = currentTenant.Name,
      };

      await _eventBus.PublishAsync(jobMessage);
    }

    private List<NotificationEntity> FilterNotifications(IEnumerable<NotificationEntity> entities)
    {
      List<NotificationEntity> result = null;
      try
      {
        bool filterDeclared = bool.TryParse(configuration["JobOptions:Whitelist:Enabled"], out bool filterEnabled);
        if (!filterDeclared || !filterEnabled)
        {
          return entities.ToList();
        }

        var modules = configuration
            .GetSection("JobOptions:Whitelist:Modules")
            ?.Get<string[]>();

        if (!modules.IsNullOrEmpty() && !modules.Contains("Notification"))
        {
          return new List<NotificationEntity>();
        }

        var tenants = configuration
            .GetSection("JobOptions:Whitelist:Tenants")
            ?.Get<string[]>()
            ?.Select<string, Guid?>(tenant => tenant.ToLower() == "host" ? null : Guid.Parse(tenant));

        var envId = configuration.GetValue<string>("EnvironmentId", null);

        result = entities
            .WhereIf(!tenants.IsNullOrEmpty(), notification => tenants.Contains(notification.TenantId))
            .Where(n => n.EnvironmentId == envId)
            .ToList();
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

    #endregion
  }
}
