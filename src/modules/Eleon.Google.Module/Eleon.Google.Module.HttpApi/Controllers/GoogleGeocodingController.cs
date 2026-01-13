using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Google.Module.Geocoding;
using VPortal.Google.Module.Models;

namespace VPortal.Google.Module.Controllers
{
  [Area(GoogleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = GoogleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Google/Geocoding")]
  public class GoogleGeocodingController : GoogleController, IGoogleGeocodingAppService
  {
    private readonly IGoogleGeocodingAppService appService;
    private readonly IVportalLogger<GoogleGeocodingController> _logger;

    public GoogleGeocodingController(
        IGoogleGeocodingAppService appService,
        IVportalLogger<GoogleGeocodingController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("GeocodeAddresses")]
    public async Task<List<LatLng>> GeocodeAddresses(List<string> addresses)
    {

      var response = await appService.GeocodeAddresses(addresses);

      return response;
    }
  }
}
