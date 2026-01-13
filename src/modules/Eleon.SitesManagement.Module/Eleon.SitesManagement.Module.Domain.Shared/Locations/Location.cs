using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
public class Location
{
  public string Path { get; set; }
  public LocationType Type { get; set; }
  public string SourceUrl { get; set; }
  public string DefaultRedirect { get; set; }
  public string ResourceId { get; set; }
  public bool IsAuthorized { get; set; } = false;
  public string? RequiredPolicy { get; set; }
  public List<Location> SubLocations { get; set; } = new List<Location>();
}


