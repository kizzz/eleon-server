using Common.EventBus.Module;
using EleonsoftSdk.modules.Messaging.Module.Messages.SystemHealth;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.EventServices;
public class SystemHealthSettingsEventHandler : IDistributedEventHandler<SystemHealthSettingsRequestMsg>, ITransientDependency
{
  private readonly TenantSettingDomainService _tenantSettingDomainService;
  private readonly IResponseContext _responseContext;
  private readonly IVportalLogger<SystemHealthSettingsEventHandler> _logger;

  public SystemHealthSettingsEventHandler(TenantSettingDomainService tenantSettingDomainService, IResponseContext responseContext, IVportalLogger<SystemHealthSettingsEventHandler> logger)
  {
    _tenantSettingDomainService = tenantSettingDomainService;
    _responseContext = responseContext;
    _logger = logger;
  }

  public async Task HandleEventAsync(SystemHealthSettingsRequestMsg eventData)
  {

    var response = new SystemHealthSettingsResponseMsg
    {
      Telemetry = null,
    };

    try
    {
      var setting = await _tenantSettingDomainService.GetTenantSystemHealthSettings();

      response.Telemetry = setting.Telemetry.ToOptions();
      response.StorageProviderId = setting.Telemetry.StorageProviderId;
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
