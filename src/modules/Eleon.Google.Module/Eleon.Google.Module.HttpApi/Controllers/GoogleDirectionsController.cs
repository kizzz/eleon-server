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
  [Route("api/Google/Direction")]
  public class GoogleDirectionsController : GoogleController, IGoogleDirectionsAppService
  {
    private readonly IGoogleDirectionsAppService appService;
    private readonly IVportalLogger<GoogleDirectionsController> _logger;

    public GoogleDirectionsController(
        IGoogleDirectionsAppService appService,
        IVportalLogger<GoogleDirectionsController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("GetDirections")]
    public async Task<DirectionsPath> GetDirections(List<LatLng> waypoints)
    {

      var response = await appService.GetDirections(waypoints);

      return response;
    }
  }
}
