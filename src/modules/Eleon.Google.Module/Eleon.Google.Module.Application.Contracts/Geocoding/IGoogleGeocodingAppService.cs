using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Google.Module.Geocoding
{
  public interface IGoogleGeocodingAppService : IApplicationService
  {
    Task<List<Models.LatLng>> GeocodeAddresses(List<string> addresses);
  }
}
