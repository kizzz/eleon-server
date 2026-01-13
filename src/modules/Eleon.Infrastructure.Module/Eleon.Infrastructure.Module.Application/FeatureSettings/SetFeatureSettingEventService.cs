using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Core.Infrastructure.Module;
using VPortal.Core.Infrastructure.Module.Entities;

namespace Core.Infrastructure.Module.FeatureSettings
{
  public class SetFeatureSettingEventService :
      IDistributedEventHandler<SetFeatureSettingMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<SetFeatureSettingEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly FeatureSettingDomainService domainService;

    public SetFeatureSettingEventService(
        IVportalLogger<SetFeatureSettingEventService> logger,
        IResponseContext responseContext,
        FeatureSettingDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(SetFeatureSettingMsg eventData)
    {
      var msg = eventData;
      var response = new ActionCompletedMsg();
      try
      {
        await domainService.SetAsync(
            msg.SettingTenantId,
            [new FeatureSettingEntity(msg.SettingGroup, msg.SettingKey, msg.Value, msg.Type, false, false, msg.SettingTenantId)]);
        response.Success = true;
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
