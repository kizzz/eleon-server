using Common.EventBus.Module;
using Eleon.InternalCommons.Lib.Messages.Locations;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using VPortal.SitesManagement.Module.Managers;

namespace Eleon.SitesManagement.Module.Eleon.SitesManagement.Module.Application.EventServices;
public class GetLocationsEventHandler : IDistributedEventHandler<GetLocationsRequestMsg>, ITransientDependency
{
  private readonly IResponseContext _responseContext;
  private readonly IVportalLogger<GetLocationsEventHandler> _logger;
  private readonly ModuleSettingsManager _moduleSettingsManager;
  private readonly ICurrentTenant _currentTenant;

  public GetLocationsEventHandler(IResponseContext responseContext, IVportalLogger<GetLocationsEventHandler> logger, ModuleSettingsManager moduleSettingsManager, ICurrentTenant currentTenant)
  {
    _responseContext = responseContext;
    _logger = logger;
    _moduleSettingsManager = moduleSettingsManager;
    _currentTenant = currentTenant;
  }

  public async Task HandleEventAsync(GetLocationsRequestMsg eventData)
  {
    var response = new GetLocationsResponseMsg { Locations = new List<LocationEto>() };

    try
    {
      using (_currentTenant.Change(eventData.TenantId))
      {
        var locations = await _moduleSettingsManager.GetLocationsAsync();
        response.Locations = locations.Select(ToEto).ToList();
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  private LocationEto ToEto(ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations.Location location)
  {
    return new LocationEto
    {
      DefaultRedirect = location.DefaultRedirect,
      IsAuthorized = location.IsAuthorized,
      Path = location.Path,
      ResourceId = location.ResourceId,
      RequiredPolicy = location.RequiredPolicy,
      SourceUrl = location.SourceUrl,
      Type = location.Type,
      SubLocations = location.SubLocations?.Select(ToEto).ToList() ?? new List<LocationEto>()
    };
  }
}
