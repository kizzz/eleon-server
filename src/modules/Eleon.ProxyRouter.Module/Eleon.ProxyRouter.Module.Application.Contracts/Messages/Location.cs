using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using ProxyRouter.Minimal.HttpApi.Models.Constants;

namespace ProxyRouter.Minimal.HttpApi.Models.Messages;


public class Location
{
  public required string Path { get; set; }
  public LocationType Type { get; set; }
  public required string SourceUrl { get; set; }
  public required string RemotePath { get; set; }
  public required string CheckedPath { get; set; }
  public required string DefaultRedirect { get; set; }
  public required string ResourceId { get; set; }
  public bool IsAuthorized { get; set; } = false;
  public string? RequiredPolicy { get; set; }
  public List<Location> SubLocations { get; set; } = new List<Location>();

  public Location Clone()
  {
    return new Location
    {
      Path = Path,
      Type = Type,
      SourceUrl = SourceUrl,
      RemotePath = RemotePath,
      DefaultRedirect = DefaultRedirect,
      ResourceId = ResourceId,
      SubLocations = SubLocations,
      CheckedPath = CheckedPath,
      IsAuthorized = IsAuthorized,
      RequiredPolicy = RequiredPolicy
    };
  }

  public override string ToString()
  {
    return $"Location: \n" +
           $"  Path: {Path}\n" +
           $"  Type: {Type}\n" +
           $"  SourceUrl: {SourceUrl}\n" +
           $"  RemotePath: {RemotePath}\n" +
           $"  CheckedPath: {CheckedPath}\n" +
           $"  DefaultRedirect: {DefaultRedirect}\n" +
           $"  ResourceId: {ResourceId}\n" +
           $"  IsAuthorized: {IsAuthorized}\n" +
           $"  RequiredPolicy: {RequiredPolicy}\n";
  }
}
