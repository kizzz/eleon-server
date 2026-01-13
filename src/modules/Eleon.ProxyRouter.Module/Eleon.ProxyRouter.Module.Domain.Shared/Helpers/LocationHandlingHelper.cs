using Eleon.ProxyRouter.Module.Eleon.ProxyRouter.Module.Domain.Shared.Helpers;
using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using ProxyRouter.Minimal.HttpApi.ErrorHandling.Exceptions;
using ProxyRouter.Minimal.HttpApi.Models.Constants;
using ProxyRouter.Minimal.HttpApi.Models.Messages;

namespace ProxyRouter.Minimal.HttpApi.Helpers;

public static class LocationHandlingHelper
{
  public static Location SelectLocation(List<Location> rootLocations, string routePath, Location currentLocation = null, string checkedPath = "/", int depth = 0)
  {
    ArgumentNullException.ThrowIfNull(routePath);
    var uncheckedPath = routePath.SafeReplaceFirst(checkedPath, "", StringComparison.CurrentCultureIgnoreCase).EnsureStartsWith("/");

    if (depth++ > 1000)
    {
      throw new Exception("Path algorithm is bad or path too big, recursion limit reached. But most likely path algorithm is bad.")
          .WithData("Status", "Critical");
    }

    var uncheckedUpper = uncheckedPath.ToUpper();
    var rootLocation = rootLocations
        .Where(l => uncheckedUpper.StartsWith((l.Path?.ToUpper() ?? "/").EnsureStartsWith("/")))
        .OrderByDescending(l => l.Path?.Length ?? 0)
        .FirstOrDefault();

    if (rootLocation == null && currentLocation == null)
    {
      throw new ProxyException($"Path not found. Route: {routePath}; Current Location: {currentLocation?.Type} {currentLocation?.Path}; Available Locations: {string.Join("; ", rootLocations.Select(l => $"Type: {l.Type}, Path: {l.Path}"))}")
          .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyResourceNotFoundError)
          .WithFriendlyMessage("Proxy hasn't found the requested route");
    }
    else if (rootLocation == null && currentLocation != null)
    {
      var result = currentLocation.Clone();
      result.CheckedPath = checkedPath;
      result.RemotePath = string.Empty;
      return result;
    }

    var newCheckedPath = checkedPath.EnsureEndsWith("/") + rootLocation.Path;

    if (rootLocation.Type != LocationType.Virtual)
    {
      var result = rootLocation.Clone();
      result.RemotePath = routePath.SafeReplaceFirst(newCheckedPath, "");
      result.CheckedPath = newCheckedPath;

      if (result.Type == LocationType.Angular && result.RemotePath.IsAngularHostFile())
      {
        result.Type = LocationType.Other;
      }

      return result;
    }

    return SelectLocation(rootLocation.SubLocations, routePath, rootLocation, newCheckedPath, depth);
  }
}
