using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

public static class UiHostingExtensions
{
  public static IApplicationBuilder UseUiHosting(
      this IApplicationBuilder app,
      IConfiguration configuration,
      ILogger? logger = null)
  {
    var opts = configuration.GetSection("UiHosting").Get<UiHostingOptions>() ?? new UiHostingOptions();

    foreach (var m in opts.Mounts ?? [])
    {
      var path = NormalizePath(m.Path);
      var abs = Path.GetFullPath(m.PhysicalPath ?? string.Empty, AppContext.BaseDirectory);

      if (!Directory.Exists(abs))
      {
        logger?.LogWarning("UI static files directory not found: {Abs} (path {Path})", abs, path);
        continue;
      }

      // 1) Serve static files
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(abs),
        RequestPath = path,
        OnPrepareResponse = ctx =>
        {
          // Cache all static assets (bundles, images, etc.)
          var seconds = Math.Max(0, (opts.CacheDays ?? 7) * 24 * 60 * 60);
          ctx.Context.Response.Headers.CacheControl = $"public,max-age={seconds}";
        }
      });

      // 2) Fallback (optional)
      app.MapWhen(
          c =>
          {
            return c.Request.Path.StartsWithSegments(path, out var remaining) && !(opts.ExcludedPaths?.Any(x => c.Request.Path.StartsWithSegments(NormalizePath(x), out _)) == true);
          },
          branch =>
          {
            var hasFallback = !string.IsNullOrWhiteSpace(m.FallbackPath);
            string? fallbackAbs = null;

            if (hasFallback)
            {
              // Support absolute file path OR relative to PhysicalPath
              fallbackAbs = Path.IsPathRooted(m.FallbackPath!)
                        ? m.FallbackPath!
                        : Path.Combine(abs, m.FallbackPath!);
            }

            // If fallback not set or file does not exist -> 404
            if (!hasFallback || !File.Exists(fallbackAbs!))
            {
              if (hasFallback && !File.Exists(fallbackAbs!))
                logger?.LogWarning("Fallback file not found for mount {Path}: {Fallback}", path, fallbackAbs);

              branch.Run(async http =>
                    {
                    http.Response.StatusCode = StatusCodes.Status404NotFound;
                    await http.Response.WriteAsync("Not Found");
                  });
              return;
            }

            // Serve fallback file (e.g., SPA index.html)
            branch.Run(async http =>
                  {
                http.Response.Headers.CacheControl = "no-store, no-cache";
                await http.Response.SendFileAsync(fallbackAbs!);
              });
          });

      logger?.LogDebug("UI mount at {Path} → {Abs} (fallback: {Fallback})",
          path, abs, string.IsNullOrWhiteSpace(m.FallbackPath) ? "none" : m.FallbackPath);
    }

    return app;
  }

  private static PathString NormalizePath(string? p)
  {
    if (string.IsNullOrWhiteSpace(p)) return PathString.Empty;
    if (!p.StartsWith('/')) p = "/" + p;
    if (p.Length > 1 && p.EndsWith('/')) p = p[..^1];
    return new PathString(p);
  }
}

public class UiHostingOptions
{
  public int? CacheDays { get; set; } = 7;
  public List<UiMount> Mounts { get; set; } = new();
  public List<string> ExcludedPaths { get; set; } = new List<string>();
}

public class UiMount
{
  /// <summary>Request path, e.g. "/ui/core" or "/assets/icons".</summary>
  public string Path { get; set; } = default!;

  /// <summary>Physical folder with built files, e.g. "dist/apps/core".</summary>
  public string PhysicalPath { get; set; } = default!;

  /// <summary>
  /// Optional fallback file path for “virtual routes”.
  /// If null/empty → return 404 for non-file requests.
  /// If set → serve this file. Can be absolute, or relative to <see cref="PhysicalPath"/>.
  /// Example: "index.html"
  /// </summary>
  public string? FallbackPath { get; set; }
}
