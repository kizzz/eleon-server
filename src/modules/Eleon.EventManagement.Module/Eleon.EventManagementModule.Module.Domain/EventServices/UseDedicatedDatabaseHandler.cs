using Common.EventBus.Module;
using EventManagementModule.Domain.EventServices;
using EventManagementModule.Module.Domain.Shared.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace ModuleCollector.EventManagementModule.EventManagementModule.Module.Domain.EventServices;
public class UseDedicatedDatabaseHandler : IDistributedEventHandler<UseDedicatedMsg>, ITransientDependency
{
  private readonly IVportalLogger<UseDedicatedDatabaseHandler> logger;
  private readonly IResponseContext responseContext;
  private readonly IUnitOfWorkManager unitOfWorkManager;

  protected ICurrentTenant CurrentTenant { get; }

  public UseDedicatedDatabaseHandler(
      IVportalLogger<UseDedicatedDatabaseHandler> logger,
      IResponseContext responseContext,
      IUnitOfWorkManager unitOfWorkManager,
      ICurrentTenant currentTenant)
  {
    this.logger = logger;
    this.responseContext = responseContext;
    this.unitOfWorkManager = unitOfWorkManager;
    CurrentTenant = currentTenant;
  }

  public async Task HandleEventAsync(UseDedicatedMsg eventData)
  {
    UseDedicatedGotMsg response;
    try
    {
      using (CurrentTenant.Change(eventData.TenantId))
      {
        using var uow = unitOfWorkManager.Begin();
        var queueDefinitionDomainService = uow.ServiceProvider.GetRequiredService<QueueDefinitionDomainService>();

        if (eventData.UseDedicated)
        {
          var queue = await queueDefinitionDomainService.EnsureCreatedAsync(
              GetQueueName(eventData.ApplicationName),
              "Created:EleoncoreTenantEntity",
              EventManagementDefaults.DefaultSystemQueueLimit
              );
        }
        else
        {
          var queue = await queueDefinitionDomainService.GetAsync(GetQueueName(eventData.ApplicationName));
          if (queue != null)
          {
            await queueDefinitionDomainService.DeleteAsync(queue.Id);
          }
        }

        await uow.SaveChangesAsync();

        response = new UseDedicatedGotMsg
        {
          Success = true
        };
      }
    }
    catch (Exception ex)
    {
      var message = eventData.UseDedicated ?
          "An exception has occured while creating queue and forwarder for application: {appName}" :
          "An exception has occured while removing queue and forwarder for application: {appName}";
      logger.Log.LogError(ex, message, eventData.ApplicationName);
      response = new UseDedicatedGotMsg
      {
        Success = false
      };
    }

    await responseContext.RespondAsync(response);
  }

  private static string GetQueueName(string appName) => appName + "TenantCreated";
}
