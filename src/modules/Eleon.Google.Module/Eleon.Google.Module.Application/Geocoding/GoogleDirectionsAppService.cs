using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.Google.Module.DomainServices;
using VPortal.Google.Module.Models;

namespace VPortal.Google.Module.Geocoding
{
  public class GoogleDirectionsAppService : GoogleAppService, IGoogleDirectionsAppService
  {
    private readonly IVportalLogger<GoogleDirectionsAppService> logger;
    private readonly GoogleDirectionsDomainService domainService;

    public GoogleDirectionsAppService(
        IVportalLogger<GoogleDirectionsAppService> logger,
        GoogleDirectionsDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<DirectionsPath> GetDirections(List<LatLng> waypoints)
    {
      DirectionsPath result = null;
      try
      {
        result = await domainService.GetDirections(waypoints);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
