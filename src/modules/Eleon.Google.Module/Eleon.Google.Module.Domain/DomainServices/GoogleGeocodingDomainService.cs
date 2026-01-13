using Common.Module.Extensions;
using Google.Module.Domain.Shared.Extensions;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Maps.Geocoding.Address.Request;
using Logging.Module;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Google.Module.Configuration;

namespace VPortal.Google.Module.DomainServices
{

  public class GoogleGeocodingDomainService : DomainService
  {
    private static int MAX_DIRECTIONS_WAYPOINTS = 27;
    private readonly IVportalLogger<GoogleGeocodingDomainService> logger;
    private readonly GoogleKeyProvider keyProvider;

    public GoogleGeocodingDomainService(
        IVportalLogger<GoogleGeocodingDomainService> logger,
        GoogleKeyProvider keyProvider)
    {
      this.logger = logger;
      this.keyProvider = keyProvider;
    }

    public async Task<List<Models.LatLng>> GeocodeAddresses(List<string> addresses)
    {
      List<Models.LatLng> result = null;
      try
      {
        if (addresses.Count == 1)
        {
          result = await GeocodeAddressAsList(addresses[0]);
        }
        else if (addresses.Count > 1)
        {
          var emptyIndices = addresses.Where(x => x.IsNullOrWhiteSpace()).Select((_, ix) => ix).ToList();

          var geocodeTasks = addresses
              .Where(x => !x.IsNullOrWhiteSpace())
              .Partition(MAX_DIRECTIONS_WAYPOINTS)
              .Select(x => x.ToList())
              .Select(x => x.Count > 1 ? BatchGeocodeAddresses(x.ToList()) : GeocodeAddressAsList(x[0]))
              .ToList();

          await Task.WhenAll(geocodeTasks);

          result = geocodeTasks.SelectMany(x => x.Result).ToList();
          foreach (var emptyIx in emptyIndices)
          {
            result.Insert(emptyIx, null);
          }
        }
        else
        {
          return new List<Models.LatLng>();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private async Task<List<Models.LatLng>> GeocodeAddressAsList(string address)
        => new List<Models.LatLng>() { await GeocodeAddress(address) };

    private async Task<List<Models.LatLng>> BatchGeocodeAddresses(List<string> addresses)
    {
      List<Models.LatLng> result = null;
      try
      {
        if (addresses.Count < 2)
        {
          throw new Exception("Batch should consist of more than 2 addresses.");
        }

        var request = new DirectionsRequest()
        {
          TravelMode = GoogleApi.Entities.Maps.Common.Enums.TravelMode.DRIVING,
          Origin = AddressToGoogleLocation(addresses[0]),
          WayPoints = addresses
                .Skip(1)
                .Take(addresses.Count - 2)
                .Select(AddressToGoogleLocation)
                .Select(x => new WayPoint(x))
                .ToList(),
          Destination = AddressToGoogleLocation(addresses[^1]),
          Key = await keyProvider.GetMapsKey(),
        };

        var response = await GoogleMaps.Directions.QueryAsync(request);
        var bestRoute = response.Routes.FirstOrDefault();
        if (bestRoute == null)
        {
          var geocoded = await addresses.Select(GeocodeAddress).WhenAll();
          result = geocoded.Select(x => x == null ? null : new Models.LatLng(x.Latitude, x.Longitude)).ToList();
        }
        else
        {
          var legs = bestRoute.Legs.ToList();

          var coordinates = response.WayPoints.Select((x, ix) =>
          {
            if (x.Status != Status.Ok)
            {
              return null;
            }

            if (ix == 0)
            {
              return legs[0].StartLocation;
            }

            return legs[ix - 1].EndLocation;
          }).ToList();

          result = coordinates
              .Select(x => new Models.LatLng(x.Latitude, x.Longitude))
              .ToList();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private async Task<Models.LatLng> GeocodeAddress(string address)
    {
      Models.LatLng result = null;
      try
      {
        if (!address.IsNullOrWhiteSpace())
        {
          var request = new AddressGeocodeRequest()
          {
            Address = address,
            Language = GetLanguage(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
            Key = await keyProvider.GetMapsKey(),
          };

          var response = await GoogleMaps.Geocode.AddressGeocode.QueryAsync(request);
          if (response.Status == Status.Ok)
          {
            var lat = (double)response.Results.FirstOrDefault()?.Geometry.Location.Latitude;
            var lng = (double)response.Results.FirstOrDefault()?.Geometry.Location.Longitude;
            result = new Models.LatLng(lat, lng);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private static LocationEx AddressToGoogleLocation(string address)
        => new LocationEx(new Address(address));

    private static Language GetLanguage(string isoCode)
    {
      var values = Enum.GetValues<Language>();
      return values.FirstOrDefault(x => isoCode == x.GetAttribute<System.Runtime.Serialization.EnumMemberAttribute>().Value);
    }
  }
}
