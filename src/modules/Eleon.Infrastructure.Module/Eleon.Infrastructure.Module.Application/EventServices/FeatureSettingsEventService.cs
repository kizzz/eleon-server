using Common.EventBus.Module;
using Common.Module.Events;
using Commons.Module.Messages.Features;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Core.Infrastructure.Module;

namespace InfratructureModule.Infrastructure.Module.Application.EventServices;
public class FeatureSettingsEventService : IDistributedEventHandler<GetFeatureSettingMsg>, ITransientDependency
{
  private readonly IVportalLogger<FeatureSettingsEventService> _logger;
  private readonly FeatureSettingDomainService _featureSettingDomainService;
  private readonly IResponseContext _responseContext;

  public FeatureSettingsEventService(
      IVportalLogger<FeatureSettingsEventService> logger,
      FeatureSettingDomainService featureSettingDomainService,
      IResponseContext responseContext)
  {
    _logger = logger;
    _featureSettingDomainService = featureSettingDomainService;
    _responseContext = responseContext;
  }

  public async Task HandleEventAsync(GetFeatureSettingMsg eventData)
  {

    var response = new GetFeatureSettingResponseMsg();

    try
    {
      var result = await _featureSettingDomainService.GetAsync(eventData.TenantId, eventData.Group, eventData.Key);
      response.Value = result?.Value;
      response.Type = result?.Type;
      response.IsEncrypted = result?.IsEncrypted ?? false;
      response.IsRequired = result?.IsRequired ?? false;
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
