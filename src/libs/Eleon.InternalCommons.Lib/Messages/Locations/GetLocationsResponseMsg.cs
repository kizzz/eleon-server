using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.InternalCommons.Lib.Messages.Locations;
public class GetLocationsResponseMsg
{
  public List<LocationEto> Locations { get; set; } = new List<LocationEto>();
}

public class LocationEto
{
  public string Path { get; set; }
  public LocationType Type { get; set; }
  public string SourceUrl { get; set; }
  public string DefaultRedirect { get; set; }
  public string ResourceId { get; set; }
  public bool IsAuthorized { get; set; } = false;
  public string? RequiredPolicy { get; set; }
  public List<LocationEto> SubLocations { get; set; } = new List<LocationEto>();
}
