using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Json;
using Volo.Abp.Uow;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.Notificators;
using VPortal.Notificator.Module.Repositories;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers
{
  public class NotificationMananger : DomainService
  {
    private static readonly string[] DefaultRunImmidiateTypes = new[] { "Push" };

    private readonly IVportalLogger<NotificationMananger> logger;
    private readonly INotificationsRepository notificationsRepository;
    private readonly NotificatorDomainService notificatorDomainService;
    private readonly IJsonSerializer jsonSerializer;
    private readonly IConfiguration configuration;
    private readonly UnitOfWorkManager unitOfWorkManager;

    public NotificationMananger(
        IVportalLogger<NotificationMananger> logger,
        INotificationsRepository notificationsRepository,
        NotificatorDomainService notificatorDomainService,
        IJsonSerializer jsonSerializer,
        IConfiguration configuration,
        UnitOfWorkManager unitOfWorkManager)
    {
      this.logger = logger;
      this.notificationsRepository = notificationsRepository;
      this.notificatorDomainService = notificatorDomainService;
      this.jsonSerializer = jsonSerializer;
      this.configuration = configuration;
      this.unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<Guid> SendAsync(EleonsoftNotification notification)
    {
      try
      {
        using var uow = unitOfWorkManager.Begin(true);

        var entityId = (notification.Id == null || notification.Id == Guid.Empty) ? GuidGenerator.Create() : notification.Id.Value;
        var isRunImmidiate = notification.RunImmidiate == true || (notification.RunImmidiate == null && DefaultRunImmidiateTypes.Contains(notification.Type.Type));

        var entity = new NotificationEntity(entityId)
        {
          Message = notification.Message,
          IsActive = !isRunImmidiate,
          Receivers = jsonSerializer.Serialize(notification.Recipients),
          Type = notification.Type,
          EnvironmentId = configuration.GetValue<string>("EnvironmentId", null),
        };
        await notificationsRepository.InsertAsync(entity);

        if (isRunImmidiate)
        {
          await notificatorDomainService.NotifyAsync(notification);
        }

        await uow.SaveChangesAsync();
        await uow.CompleteAsync();

        return entity.Id;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }
  }
}
