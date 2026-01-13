using ProxyRouter.Minimal.HttpApi.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

public enum MergeStrategy
{
  PerfereFirst,
  PrefereSecond,
  // You can add more later (e.g. Custom, ThrowOnConflict, etc.)
}

public static class LocationsMerger
{
  public static List<Location> MergeLocations(
      List<Location> first,
      List<Location> second,
      MergeStrategy mergeStrategy = MergeStrategy.PerfereFirst)
  {
    first ??= new List<Location>();
    second ??= new List<Location>();

    var firstDict = first
        .Where(x => x != null)
        .ToDictionary(x => NormalizePath(x.Path), x => x, StringComparer.OrdinalIgnoreCase);

    var secondDict = second
        .Where(x => x != null)
        .ToDictionary(x => NormalizePath(x.Path), x => x, StringComparer.OrdinalIgnoreCase);

    // All unique keys from both
    var allKeys = new HashSet<string>(firstDict.Keys, StringComparer.OrdinalIgnoreCase);
    allKeys.UnionWith(secondDict.Keys);

    var result = new List<Location>();

    foreach (var key in allKeys)
    {
      firstDict.TryGetValue(key, out var loc1);
      secondDict.TryGetValue(key, out var loc2);

      if (loc1 != null && loc2 != null)
      {
        // Both collections have this path
        var preferred = mergeStrategy switch
        {
          MergeStrategy.PerfereFirst => loc1,
          MergeStrategy.PrefereSecond => loc2,
          _ => loc1
        };

        var other = ReferenceEquals(preferred, loc1) ? loc2 : loc1;

        // Clone preferred WITHOUT sublocations first
        var mergedLocation = CloneWithoutSubLocations(preferred);

        // Recursively merge sublocations by path
        mergedLocation.SubLocations = MergeLocations(
            preferred.SubLocations ?? new List<Location>(),
            other.SubLocations ?? new List<Location>(),
            mergeStrategy);

        result.Add(mergedLocation);
      }
      else if (loc1 != null)
      {
        // Only first has this path
        result.Add(CloneDeep(loc1));
      }
      else if (loc2 != null)
      {
        // Only second has this path
        result.Add(CloneDeep(loc2));
      }
    }

    return result;
  }

  // === Helpers ===

  private static string NormalizePath(string path)
  {
    // Ignore case, trim spaces and leading/trailing slashes
    if (string.IsNullOrWhiteSpace(path))
      return string.Empty;

    return path.Trim(' ', '/').ToLowerInvariant();
  }

  private static Location CloneWithoutSubLocations(Location src)
  {
    return new Location
    {
      Path = src.Path,
      Type = src.Type,
      SourceUrl = src.SourceUrl,
      RemotePath = src.RemotePath,
      CheckedPath = src.CheckedPath,
      DefaultRedirect = src.DefaultRedirect,
      ResourceId = src.ResourceId,
      IsAuthorized = src.IsAuthorized,
      RequiredPolicy = src.RequiredPolicy,
      SubLocations = new List<Location>() // empty, we will fill merged children
    };
  }

  private static Location CloneDeep(Location src)
  {
    var clone = CloneWithoutSubLocations(src);
    foreach (var sub in src.SubLocations ?? Enumerable.Empty<Location>())
    {
      clone.SubLocations.Add(CloneDeep(sub));
    }
    return clone;
  }
}
