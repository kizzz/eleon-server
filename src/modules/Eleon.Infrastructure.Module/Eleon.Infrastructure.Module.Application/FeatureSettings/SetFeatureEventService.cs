using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.FeatureManagement;

namespace Core.Infrastructure.Module.FeatureSettings
{
  public class SetFeatureEventService :
      IDistributedEventHandler<SetFeatureMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<SetFeatureEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly IFeatureManager featureManager;

    public SetFeatureEventService(
        IVportalLogger<SetFeatureEventService> logger,
        IResponseContext responseContext,
        IFeatureManager featureManager)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.featureManager = featureManager;
    }

    public async Task HandleEventAsync(SetFeatureMsg eventData)
    {
      var msg = eventData;
      var response = new ActionCompletedMsg();
      try
      {
        await featureManager.SetForTenantAsync(eventData.FeatureTenantId, eventData.FeatureName, eventData.FeatureValue);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
