using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Google.Module.Models;

namespace VPortal.Google.Module.Geocoding
{
  public interface IGoogleDirectionsAppService : IApplicationService
  {
    Task<DirectionsPath> GetDirections(List<Models.LatLng> waypoints);
  }
}
