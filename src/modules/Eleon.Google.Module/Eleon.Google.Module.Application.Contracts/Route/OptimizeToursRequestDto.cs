using Messaging.Module.ETO.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Google.Module.Google.Module.Application.Contracts.Route;
public class OptimizeToursRequestDto
{
  public List<RouteShipmentDto> Shipments { get; set; } = new List<RouteShipmentDto>();
  public List<RouteVehicleDto> Vehicles { get; set; } = new List<RouteVehicleDto>();
  public List<RouteShipmentTypeRequirementDto> ShipmentTypeRequirements { get; set; } = new List<RouteShipmentTypeRequirementDto>();
  public DateTime GlobalStartTime { get; set; }
  public DateTime GlobalEndTime { get; set; }
}
