using Common.EventBus.Module;
using Common.Module.Constants;
using Google.Cloud.Optimization.V1;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using Logging.Module;
using Messaging.Module.ETO.Google;
using Messaging.Module.Messages;
using ModuleCollector.Google.Module.Google.Module.Application.Contracts.OptimizeRoute;
using ModuleCollector.Google.Module.Google.Module.Application.Contracts.Route;
using VPortal.Google.Module.DomainServices;

namespace ModuleCollector.Google.Module.Google.Module.Application.OptimizeRoute;
public class RouteAppService : VPortal.Google.Module.GoogleAppService, IRouteAppService
{
  private readonly IVportalLogger<RouteAppService> logger;
  private readonly IResponseContext responseContext;
  private readonly GoogleFleetRoutingDomainService fleetRoutingDomainService;

  public RouteAppService(
      IVportalLogger<RouteAppService> logger,
      IResponseContext responseContext,
      GoogleFleetRoutingDomainService fleetRoutingDomainService)
  {
    this.logger = logger;
    this.responseContext = responseContext;
    this.fleetRoutingDomainService = fleetRoutingDomainService;
  }

  public async Task<OptimizedToursDto> OptimizeRouteAsync(OptimizeToursRequestDto model)
  {
    OptimizedToursDto result = null;
    try
    {
      var shipments = model.Shipments.Select(x => new Shipment()
      {
        ShipmentType = x.ShipmentType,
        Label = x.Label,
        Deliveries =
                    {
                        x.Deliveries.Select(x => new Shipment.Types.VisitRequest
                        {
                            ArrivalLocation = new LatLng()
                            {
                                Latitude = x.ArrivalLocation.Latitude,
                                Longitude = x.ArrivalLocation.Longitude,
                            },
                            Duration = new Duration()
                            {
                                Seconds = x.Duration.Seconds,
                                Nanos = 0,
                            },
                        }),
                    },
        Pickups =
                    {
                        x.Pickups.Select(x => new Shipment.Types.VisitRequest
                        {
                            ArrivalLocation = new LatLng()
                            {
                                Latitude = x.ArrivalLocation.Latitude,
                                Longitude = x.ArrivalLocation.Longitude,
                            },
                            Duration = new Duration()
                            {
                                Seconds = x.Duration.Seconds,
                                Nanos = 0,
                            },
                        }),
                    },
      });

      var request = new OptimizeToursRequest()
      {
        Model = new ShipmentModel()
        {
          Shipments = { shipments },
          Vehicles =
                        {
                            model.Vehicles.Select(x => new Vehicle()
                            {
                                Label = x.Label,
                                FixedCost = x.FixedCost,
                                TravelMode = Vehicle.Types.TravelMode.Driving,
                            }),
                        },
          ShipmentTypeRequirements =
                        {
                            model.ShipmentTypeRequirements.Select(x => new ShipmentTypeRequirement
                            {
                                DependentShipmentTypes = { x.DependentShipmentTypes },
                                RequiredShipmentTypeAlternatives = { x.RequiredShipmentTypeAlternatives },
                                RequirementMode = x.RequirementMode switch
                                {
                                    RouteShipmentTypeRequirementMode.InSameVehicleAtDeliveryTime => ShipmentTypeRequirement.Types.RequirementMode.InSameVehicleAtDeliveryTime,
                                    RouteShipmentTypeRequirementMode.InSameVehicleAtPickupTime => ShipmentTypeRequirement.Types.RequirementMode.InSameVehicleAtPickupTime,
                                    RouteShipmentTypeRequirementMode.PerformedBySameVehicle => ShipmentTypeRequirement.Types.RequirementMode.PerformedBySameVehicle,
                                    _ => ShipmentTypeRequirement.Types.RequirementMode.Unspecified,
                                },
                            }),
                        },
          GlobalStartTime = Timestamp.FromDateTime(model.GlobalStartTime),
          GlobalEndTime = Timestamp.FromDateTime(model.GlobalEndTime),
        },
      };

      var optimized = await fleetRoutingDomainService.OptimizeRoute(request);

      result = new OptimizedToursDto()
      {
        SkippedShipmentLabels = optimized.SkippedShipments.Select(x => x.Label).ToList(),
        Routes = optimized.Routes.Select(x => new RouteDto { VehicleLabel = x.VehicleLabel, RouteVisitsShipmentLabels = x.Visits.Select(v => v.ShipmentLabel).ToList() }).ToList(),
      };
    }
    catch (Exception ex)
    {
      logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }

    return result;
  }
}
