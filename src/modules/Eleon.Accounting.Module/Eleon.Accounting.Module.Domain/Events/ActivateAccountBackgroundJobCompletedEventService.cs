using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using Logging.Module;
using Messaging.Module.Messages;
using ModuleCollector.Accounting.Module.Accounting.Module.Domain.Shared.Constants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Accounting.Module.DomainServices;

namespace VPortal.Accounting.Module.EventServices
{
  public class ActivateAccountBackgroundJobCompletedEventService :
      IDistributedEventHandler<BackgroundJobExecutionCompletedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ActivateAccountBackgroundJobCompletedEventService> logger;
    private readonly AccountDomainService accountDomainService;
    private readonly XmlSerializerHelper xmlSerializerHelper;

    public ActivateAccountBackgroundJobCompletedEventService(
        IVportalLogger<ActivateAccountBackgroundJobCompletedEventService> logger,
        AccountDomainService accountDomainService,
        XmlSerializerHelper xmlSerializerHelper)
    {
      this.logger = logger;
      this.accountDomainService = accountDomainService;
      this.xmlSerializerHelper = xmlSerializerHelper;
    }

    public async Task HandleEventAsync(BackgroundJobExecutionCompletedMsg eventData)
    {
      if (eventData.Type != AccountingBackgroundJobTypes.ActivateAccountTenant)
      {
        return;
      }

      bool errored = false;
      AccountValueObject accountValueObject = new();
      try
      {
        accountValueObject = xmlSerializerHelper.DeserializeFromXml<AccountValueObject>(eventData.ParamsForRetryExecution);
        if (eventData.Status == BackgroundJobExecutionStatus.Completed)
        {
          if (accountValueObject.Id.HasValue)
          {
            await accountDomainService.ExecuteActivateAccount(accountValueObject.Id.Value);
          }
          else
          {
            errored = true;
          }
        }
        else
        {
          errored = true;
        }
      }
      catch (Exception ex)
      {
        errored = true;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }
    }
  }
}
