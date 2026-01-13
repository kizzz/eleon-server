using Messaging.Module.ETO.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Google.Module.Google.Module.Application.Contracts.Route;
public class OptimizedToursDto
{
  public List<string> SkippedShipmentLabels { get; set; }
  public List<RouteDto> Routes { get; set; }
}

public class RouteDto
{
  public string VehicleLabel { get; set; }
  public List<string> RouteVisitsShipmentLabels { get; set; }
}
