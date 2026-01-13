using GoogleApi;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.Google.Module.Configuration;
using VPortal.Google.Module.Models;

namespace VPortal.Google.Module.DomainServices
{
  public class GoogleDirectionsDomainService : DomainService
  {
    private const int MAX_DIRECTIONS_WAYPOINTS = 27;

    private readonly IVportalLogger<GoogleDirectionsDomainService> logger;
    private readonly GoogleKeyProvider keyProvider;

    public GoogleDirectionsDomainService(
        IVportalLogger<GoogleDirectionsDomainService> logger,
        GoogleKeyProvider keyProvider)
    {
      this.logger = logger;
      this.keyProvider = keyProvider;
    }

    public async Task<DirectionsPath> GetDirections(List<Models.LatLng> waypoints)
    {
      DirectionsPath result = null;
      try
      {
        if (waypoints.Count > MAX_DIRECTIONS_WAYPOINTS)
        {
          return null;
        }

        var request = new DirectionsRequest()
        {
          TravelMode = GoogleApi.Entities.Maps.Common.Enums.TravelMode.DRIVING,
          Origin = LatLngToGoogleLocation(waypoints[0]),
          WayPoints = waypoints
                .Skip(1)
                .Take(waypoints.Count - 2)
                .Select(LatLngToGoogleLocation)
                .Select(x => new WayPoint(x))
                .ToList(),
          Destination = LatLngToGoogleLocation(waypoints[^1]),
          Key = await keyProvider.GetMapsKey(),
        };

        var response = await GoogleMaps.Directions.QueryAsync(request);
        if (response.Status == GoogleApi.Entities.Common.Enums.Status.Ok)
        {

          var legs = response.Routes
              .First()
              .Legs;

          var steps = legs
              .SelectMany(x => x.Steps)
              .SelectMany(x => x.PolyLine.Line)
              .Select(x => new Models.LatLng(x.Latitude, x.Longitude))
              .ToList();

          var durationSeconds = legs.Sum(x => x.Duration.Value);
          var durationMinutes = durationSeconds / 60;
          durationSeconds %= 60;
          var durationHours = durationMinutes / 60;
          durationMinutes %= 60;

          var distanceKilometers = legs.Sum(x => x.Distance.Value) / 1000;

          result = new DirectionsPath()
          {
            DurationSeconds = durationSeconds,
            DurationMinutes = durationMinutes,
            DurationHours = durationHours,
            DistanceKilometers = distanceKilometers,
            Path = steps,
          };
        }
        else if (response.Status == GoogleApi.Entities.Common.Enums.Status.ZeroResults)
        {
          // IGNORE
        }
        else
        {
          throw new Exception($"An error occured on Google Directions API call.\nStatus: {response.Status}\nError: {response.ErrorMessage}");
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private static LocationEx LatLngToGoogleLocation(Models.LatLng latLng)
        => new LocationEx(new CoordinateEx(latLng.Latitude, latLng.Longitude));
  }
}
