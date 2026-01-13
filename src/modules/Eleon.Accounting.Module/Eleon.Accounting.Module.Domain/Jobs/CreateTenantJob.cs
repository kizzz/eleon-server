using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.BackgroundJobs;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.Accounting.Module.Accounting.Module.Domain.Shared.Constants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.Accounting.Module.Tenant;

namespace VPortal.Accounting.Module.Jobs
{

  public class CreateTenantJob :
      IDistributedEventHandler<StartingInternalSystemJobExecutionMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CreateTenantJob> logger;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IObjectMapper objectMapper;
    private readonly AccountTenantManager accountTenantManager;
    private readonly XmlSerializerHelper xmlSerializerHelper;

    public CreateTenantJob(
        IVportalLogger<CreateTenantJob> logger,
        IDistributedEventBus massTransitPublisher,
        IObjectMapper objectMapper,
        AccountTenantManager accountTenantManager,
        XmlSerializerHelper xmlSerializerHelper)
    {
      this.logger = logger;
      this.massTransitPublisher = massTransitPublisher;
      this.objectMapper = objectMapper;
      this.xmlSerializerHelper = xmlSerializerHelper;
      this.accountTenantManager = accountTenantManager;
    }

    public async Task HandleEventAsync(StartingInternalSystemJobExecutionMsg eventData)
    {
      if (eventData.BackgroundJob.Type != AccountingBackgroundJobTypes.CreateTenantFromAccount)
      {
        return;
      }


      var job = eventData.BackgroundJob;
      var execution = eventData.Execution;

      await massTransitPublisher.PublishAsync(new MarkJobExecutionStartedMsg
      {
        JobId = job.Id,
        ExecutionId = execution.Id,
        TenantId = eventData.TenantId,
        TenantName = eventData.TenantName,
      });

      string userErrorMessage = null;
      string accountTenantId = null;
      var messages = new List<BackgroundJobTextInfoEto>();
      var status = BackgroundJobExecutionStatus.Errored;

      try
      {
        var accountValueObject = xmlSerializerHelper.DeserializeFromXml<AccountValueObject>(execution.StartExecutionParams);
        var newAccountTenantId = await accountTenantManager.RequestCreateTenant(accountValueObject, job.Id);
        if (!newAccountTenantId.HasValue)
        {
          throw new Exception("Failed to create new tenant");
        }

        accountTenantId = newAccountTenantId.Value.ToString();
        status = BackgroundJobExecutionStatus.Completed;
      }
      catch (Exception ex)
      {
        userErrorMessage = ex.Message;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        if (status != BackgroundJobExecutionStatus.Completed)
        {
          messages.Add(new BackgroundJobTextInfoEto()
          {
            Type = BackgroundJobMessageType.Error,
            TextMessage = userErrorMessage,
          });
        }

        var message = new BackgroundJobExecutionCompletedMsg
        {
          Type = job.Type,
          BackgroundJobId = job.Id,
          ExecutionId = execution.Id,
          ParamsForRetryExecution = accountTenantId,
          ExtraParamsForRetryExecution = execution.StartExecutionExtraParams,
          Status = status,
          Messages = messages,
          TenantId = eventData.TenantId,
          TenantName = eventData.TenantName,
        };

        await massTransitPublisher.PublishAsync(message);

      }
    }
  }
}
