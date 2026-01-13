using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace VPortal.BackgroundJobs.Module.EventHandlers;

public class GetBackgroundJobEventService :
        IDistributedEventHandler<GetBackgroundJobEtoMsg>,
        ITransientDependency
{
  private readonly IVportalLogger<GetBackgroundJobEventService> logger;
  private readonly IBackgroundJobsRepository repository;
  private readonly IResponseContext responseContext;
  private readonly IObjectMapper objectMapper;

  public GetBackgroundJobEventService(
      IVportalLogger<GetBackgroundJobEventService> logger,
      IBackgroundJobsRepository repository,
      IResponseContext responseContext,
      IObjectMapper objectMapper)
  {
    this.logger = logger;
    this.repository = repository;
    this.responseContext = responseContext;
    this.objectMapper = objectMapper;
  }

  public async Task HandleEventAsync(GetBackgroundJobEtoMsg eventData)
  {
    try
    {
      var backgroundJobEntity = await repository.GetAsync(eventData.BackgroundJobId);
      var backgroundJobEto = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(backgroundJobEntity);
      var message = new BackgroundJobEtoGotMsg
      {
        Success = true,
        BackgroundJob = backgroundJobEto
      };

      await responseContext.RespondAsync(message);
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
