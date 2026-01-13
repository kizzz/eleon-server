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

  public class ActivateAccountTenantJob :
      IDistributedEventHandler<StartingInternalSystemJobExecutionMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ActivateAccountTenantJob> logger;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IObjectMapper objectMapper;
    private readonly AccountTenantManager accountTenantManager;
    private readonly XmlSerializerHelper xmlSerializerHelper;

    public ActivateAccountTenantJob(
        IVportalLogger<ActivateAccountTenantJob> logger,
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
      if (eventData.BackgroundJob.Type != AccountingBackgroundJobTypes.ActivateAccountTenant)
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

      var status = BackgroundJobExecutionStatus.Errored;
      string userErrorMessage = null;
      var messages = new List<BackgroundJobTextInfoEto>();

      try
      {
        var accountValueObject = xmlSerializerHelper.DeserializeFromXml<AccountValueObject>(execution.StartExecutionParams);
        await accountTenantManager.RequestActivateAccountTenant(accountValueObject, job.Id);
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
          ParamsForRetryExecution = execution.StartExecutionParams,
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
