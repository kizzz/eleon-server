using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.Google.Module.DomainServices;

namespace VPortal.Google.Module.Geocoding
{
  public class GoogleGeocodingAppService : GoogleAppService, IGoogleGeocodingAppService
  {
    private readonly IVportalLogger<GoogleGeocodingAppService> logger;
    private readonly GoogleGeocodingDomainService domainService;

    public GoogleGeocodingAppService(
        IVportalLogger<GoogleGeocodingAppService> logger,
        GoogleGeocodingDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<List<Models.LatLng>> GeocodeAddresses(List<string> addresses)
    {
      List<Models.LatLng> result = null;
      try
      {
        result = await domainService.GeocodeAddresses(addresses);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
