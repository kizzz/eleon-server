using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using Logging.Module;
using Messaging.Module.Messages;
using ModuleCollector.Accounting.Module.Accounting.Module.Domain.Shared.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Accounting.Module.DomainServices;

namespace VPortal.Accounting.Module.EventServices
{
  public class CancelAccountBackgroundJobCompletedEventService :
      IDistributedEventHandler<BackgroundJobExecutionCompletedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CancelAccountBackgroundJobCompletedEventService> logger;
    private readonly AccountDomainService accountDomainService;
    private readonly XmlSerializerHelper xmlSerializerHelper;

    public CancelAccountBackgroundJobCompletedEventService(
        IVportalLogger<CancelAccountBackgroundJobCompletedEventService> logger,
        AccountDomainService accountDomainService,
        XmlSerializerHelper xmlSerializerHelper)
    {
      this.logger = logger;
      this.xmlSerializerHelper = xmlSerializerHelper;
      this.accountDomainService = accountDomainService;
    }

    public async Task HandleEventAsync(BackgroundJobExecutionCompletedMsg eventData)
    {
      if (eventData.Type != AccountingBackgroundJobTypes.CancelAccountTenant)
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
          await accountDomainService.ExecuteCancelAccount(accountValueObject.Id.Value);
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
