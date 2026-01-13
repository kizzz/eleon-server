using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Eleon.Common.Lib.Helpers;

public static class MultiPathBaseExtensions
{
  public static IApplicationBuilder UseMultiPathBase(this IApplicationBuilder app, IConfiguration config)
  {
    // Read config (string or array)
    var section = config.GetSection("App:PathBase");

    var bases = section.Get<List<string>>() ??
                (section.Value is { Length: > 0 } single
                    ? new List<string> { single }
                    : new List<string>());

    // Normalize all path bases
    var normalized = bases
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Select(Normalize)
        .Distinct()
        .ToArray();

    if (normalized.Length == 0)
      return app; // nothing to do

    return app.Use(async (context, next) =>
    {
      var reqPath = context.Request.Path.ToString();

      foreach (var pathBase in normalized)
      {
        if (reqPath.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase))
        {
          context.Request.PathBase = pathBase;
          context.Request.Path = reqPath[pathBase.Length..];
          break;
        }
      }

      await next();
    });
  }

  private static string Normalize(string basePath)
  {
    basePath = basePath.Trim();

    if (!basePath.StartsWith("/"))
      basePath = "/" + basePath;

    return basePath.Length > 1 ? basePath.TrimEnd('/') : basePath;
  }
}
