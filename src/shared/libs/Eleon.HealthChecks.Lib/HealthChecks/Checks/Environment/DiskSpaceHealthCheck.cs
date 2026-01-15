using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;

// ------------- Options ----------------

public sealed class DiskSpaceHealthCheckOptions
{
  /// <summary>List of items to validate.</summary>
  public List<FilePathRule> Items { get; set; } = new();
}

public sealed class FilePathRule
{
  /// <summary>File or folder path. If not absolute, resolved as AppContext.BaseDirectory + Path.</summary>
  public string Path { get; set; } = default!;

  /// <summary>Maximum allowed size in MEGABYTES (file size or total folder size).</summary>
  public long MaxSizeMb { get; set; }
}

// ------------- Health Check ------------

public class DiskSpaceHealthCheck : DefaultHealthCheck
{
  private const int LargestCount = 5;

  private readonly DiskSpaceHealthCheckOptions _opt;

  public DiskSpaceHealthCheck(IServiceProvider serviceProvider, IOptions<DiskSpaceHealthCheckOptions>? options = null) : base(serviceProvider)
      => _opt = options?.Value ?? new DiskSpaceHealthCheckOptions();

  public override string Name => "DiskSpace";
  public override bool IsPublic => true;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    if (_opt.Items is not { Count: > 0 })
    {
      AddSimple(report, "Warning_NoItemsProvided", "No file/folder rules provided.", ReportInformationSeverity.Warning);
      report.Status = HealthCheckStatus.OK;
      report.Message = "No paths to check.";
    }

    bool anyExceeded = false;
    int total = 0, ok = 0, missing = 0, errors = 0;

    foreach (var rule in _opt.Items)
    {
      total++;

      var absPath = ResolvePath(rule.Path);
      var keyBase = SanitizeKey(absPath);

      var sw = Stopwatch.StartNew();
      try
      {
        if (File.Exists(absPath))
        {
          var fi = new FileInfo(absPath);
          long size = fi.Length;
          long maxSizeBytes = rule.MaxSizeMb * 1024 * 1024;

          AddJson(report, $"File_{keyBase}_Info", new
          {
            Kind = "File",
            Path = absPath,
            SizeBytes = size,
            MaxSizeMb = rule.MaxSizeMb
          });

          if (size > maxSizeBytes)
          {
            anyExceeded = true;
            string limitMsg = $"Size={size}B ({size / (1024.0 * 1024.0):F2}MB), Allowed={rule.MaxSizeMb}MB ({maxSizeBytes}B)";
            AddSimple(report, $"File_{keyBase}_Exceeded",
                $"File exceeds limit: {limitMsg}",
                ReportInformationSeverity.Error);
          }
          else
          {
            string limitMsg = $"Size={size}B ({size / (1024.0 * 1024.0):F2}MB), Allowed={rule.MaxSizeMb}MB ({maxSizeBytes}B)";
            AddSimple(report, $"File_{keyBase}_WithinLimit",
                $"File within limit: {limitMsg}");
          }

          ok++;
        }
        else if (Directory.Exists(absPath))
        {
          var (totalBytes, fileCount, largest) = ScanDirectory(absPath, LargestCount);
          long maxSizeBytes = rule.MaxSizeMb * 1024 * 1024;

          AddJson(report, $"Folder_{absPath}_Info", new
          {
            Kind = "Folder",
            Path = absPath,
            TotalSizeBytes = totalBytes,
            FilesCount = fileCount,
            MaxSizeMb = rule.MaxSizeMb
          });

          AddJson(report, $"Folder_{absPath}_Top{LargestCount}", largest.Select(x => new
          {
            Path = x.Path,
            SizeBytes = x.Size
          }).ToList());

          if (totalBytes > maxSizeBytes)
          {
            anyExceeded = true;
            string limitMsg = $"Size={totalBytes}B ({totalBytes / (1024.0 * 1024.0):F2}MB), Allowed={rule.MaxSizeMb}MB ({maxSizeBytes}B)";
            AddSimple(report, $"Folder_{absPath}_Exceeded",
                $"Folder exceeds limit: {limitMsg}",
                ReportInformationSeverity.Error);
          }
          else
          {
            string limitMsg = $"Size={totalBytes}B ({totalBytes / (1024.0 * 1024.0):F2}MB), Allowed={rule.MaxSizeMb}MB ({maxSizeBytes}B)";
            AddSimple(report, $"Folder_{absPath}_WithinLimit",
                $"Folder within limit: {limitMsg}");
          }

          ok++;
        }
        else
        {
          AddSimple(report, $"Path_{absPath}_Missing",
              $"Path not found: {absPath}",
              ReportInformationSeverity.Warning);
          missing++;
        }
      }
      catch (Exception ex)
      {
        AddJson(report, $"Path_{absPath}_Error", new
        {
          Path = absPath,
          Error = $"{ex.GetType().Name}: {ex.Message}"
        }, ReportInformationSeverity.Error);
        errors++;
      }
      finally
      {
        sw.Stop();
        AddJson(report, $"Path_{absPath}_Status", new
        {
          Path = absPath,
          ElapsedMs = sw.ElapsedMilliseconds,
          Status = "Checked"
        });
      }
    }

    report.Status = anyExceeded ? HealthCheckStatus.Failed : HealthCheckStatus.OK;
    report.Message = $"Files/folders checked: {total} total, {ok} OK, {missing} missing, {errors} errors."
                     + (anyExceeded ? " Some items exceeded configured size." : "");

    AddSimple(report, "TotalItems", total.ToString());
    AddSimple(report, "OkItems", ok.ToString());
    AddSimple(report, "MissingItems", missing.ToString(), missing > 0 ? ReportInformationSeverity.Warning : ReportInformationSeverity.Info);
    AddSimple(report, "ErrorItems", errors.ToString(), errors > 0 ? ReportInformationSeverity.Error : ReportInformationSeverity.Info);
  }

  // ------------- Helpers -------------

  private static string ResolvePath(string path)
  {
    if (string.IsNullOrWhiteSpace(path)) return path ?? string.Empty;
    return Path.IsPathRooted(path)
        ? Path.GetFullPath(path)
        : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
  }

  private static string SanitizeKey(string path)
  {
    // Turn path into a stable, key-safe token
    var key = path.Replace(':', '_').Replace('\\', '/');
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
                  .TrimEnd('=')
                  .Replace('+', '-')
                  .Replace('/', '_');
  }

  private static (long TotalBytes, long FileCount, List<(string Path, long Size)> Largest)
      ScanDirectory(string root, int largestCount)
  {
    long total = 0;
    long count = 0;
    var top = new TopNFiles(largestCount);

    var dirs = new Stack<string>();
    dirs.Push(root);

    while (dirs.Count > 0)
    {
      string dir = dirs.Pop();

      try
      {
        // Recurse into subdirectories
        foreach (var d in Directory.EnumerateDirectories(dir))
        {
          try { dirs.Push(d); } catch { /* skip on error */ }
        }

        // Files in this directory
        foreach (var f in Directory.EnumerateFiles(dir))
        {
          try
          {
            var fi = new FileInfo(f);
            long size = fi.Length;
            total += size;
            count++;
            top.Add((fi.FullName, size));
          }
          catch
          {
            // ignore inaccessible file
          }
        }
      }
      catch
      {
        // ignore inaccessible directory
      }
    }

    return (total, count, top.ToList());
  }

  private sealed class TopNFiles
  {
    private readonly int _n;
    private readonly List<(string Path, long Size)> _items;

    public TopNFiles(int n)
    {
      _n = Math.Max(1, n);
      _items = new List<(string, long)>(_n + 1);
    }

    public void Add((string Path, long Size) item)
    {
      _items.Add(item);
      // keep sorted descending by size; trim
      _items.Sort((a, b) => b.Size.CompareTo(a.Size));
      if (_items.Count > _n)
        _items.RemoveAt(_items.Count - 1);
    }

    public List<(string Path, long Size)> ToList() => new(_items);
  }

  private static void AddSimple(HealthCheckReportEto report, string key, string value,
      ReportInformationSeverity severity = ReportInformationSeverity.Info)
  {
    report.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = value,
      Severity = severity,
      Type = HealthCheckDefaults.ExtraInfoTypes.Simple
    });
  }

  private static void AddJson(HealthCheckReportEto report, string key, object value,
      ReportInformationSeverity severity = ReportInformationSeverity.Info)
  {
    report.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = JsonSerializer.Serialize(value),
      Severity = severity,
      Type = HealthCheckDefaults.ExtraInfoTypes.Json
    });
  }
}
