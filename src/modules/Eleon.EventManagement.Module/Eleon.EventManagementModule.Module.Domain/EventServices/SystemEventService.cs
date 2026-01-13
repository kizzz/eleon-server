using EventManagementModule.Module.Domain.Shared.Constants;
using EventManagementModule.Module.Domain.Shared.Entities;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace EventManagementModule.Domain.EventServices
{
  public class SystemEventService : IDistributedEventHandler<SystemEventMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<SystemEventService> logger;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly ICurrentTenant currentTenant;

    public SystemEventService(
        IVportalLogger<SystemEventService> logger,
        IUnitOfWorkManager unitOfWorkManager,
        ICurrentTenant currentTenant)
    {
      this.logger = logger;
      this.unitOfWorkManager = unitOfWorkManager;
      this.currentTenant = currentTenant;
    }
    public async Task HandleEventAsync(SystemEventMsg eventData)
    {
      try
      {
        using var uow = unitOfWorkManager.Begin(true);
        var eventDomainService = uow.ServiceProvider.GetRequiredService<EventDomainService>();

        using (currentTenant.Change(currentTenant.Id))
        {
          await eventDomainService.PublishAsync(
              EventManagementDefaults.SystemQueueName,
              eventData.MessageType,
              eventData.MessageData);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
