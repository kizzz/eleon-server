using Eleon.Common.Lib.modules.HealthCheck.Module.General;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;

public class EnvironmentHealthCheck : DefaultHealthCheck
{
  private readonly EnvironmentHealthCheckOptions _options;

  public override string Name => "Environment";
  public override bool IsPublic => true;

  public EnvironmentHealthCheck(IOptions<EnvironmentHealthCheckOptions> options, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    _options = options.Value;
  }

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    var extra = new List<ReportExtraInformationEto>();

    // -------- CPU --------
    var cpuInfo = await SampleCpuAsync(_options.CpuSampleMilliseconds);

    var topCpu = cpuInfo.PerProcess
        .OrderByDescending(p => p.CpuPercent)
        .Take(5)
        .Select(p => new
        {
          p.Pid,
          p.Name,
          CpuPercent = Math.Round(p.CpuPercent, 2),
          WorkingSetMB = Math.Round(p.WorkingSetBytes / 1024.0 / 1024.0, 2)
        }).ToList();

    // -------- Memory --------
    var totalAvailableBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
    var processes = SafeGetProcesses();
    long totalWorkingSet = processes.Sum(p => SafeWS(p));
    double memPercent = totalAvailableBytes > 0
        ? (totalWorkingSet * 100.0 / totalAvailableBytes)
        : 0.0;

    var topMem = processes
        .OrderByDescending(p => SafeWS(p))
        .Take(5)
        .Select(p => new
        {
          Pid = SafePid(p),
          Name = SafeName(p),
          WorkingSetMB = Math.Round(SafeWS(p) / 1024.0 / 1024.0, 2)
        }).ToList();

    // -------- Disk --------
    var drives = DriveInfo.GetDrives()
        .Where(d => { try { return d.IsReady && d.DriveType != DriveType.CDRom; } catch { return false; } })
        .Select(d =>
        {
          double usedPct = (d.TotalSize > 0)
                  ? (100.0 * (d.TotalSize - d.AvailableFreeSpace) / d.TotalSize)
                  : 0.0;

          return new
          {
            d.Name,
            Type = d.DriveType.ToString(),
            TotalGB = SafeToGB(d.TotalSize),
            FreeGB = SafeToGB(d.AvailableFreeSpace),
            UsedPercent = Math.Round(usedPct, 2),
            Format = SafeStr(() => d.DriveFormat)
          };
        })
        .OrderByDescending(x => x.UsedPercent)
        .ToList();

    // -------- Status --------
    bool failCpu = cpuInfo.TotalCpuPercent > _options.CpuThresholdPercent;
    bool failMem = memPercent > _options.MemoryThresholdPercent;
    bool failDisk = _options.FailOnDiskThreshold && drives.Any(d => d.UsedPercent > _options.DiskThresholdPercent);

    var status = (failCpu || failMem || failDisk) ? HealthCheckStatus.Failed : HealthCheckStatus.OK;
    var message = $"CPU: {cpuInfo.TotalCpuPercent:F2}% / {_options.CpuThresholdPercent}%, " +
                  $"Mem: {memPercent:F2}% / {_options.MemoryThresholdPercent}%, " +
                  $"Disks > {_options.DiskThresholdPercent}%: {drives.Count(d => d.UsedPercent > _options.DiskThresholdPercent)}";

    // -------- Extra Information --------
    extra.Add(new ReportExtraInformationEto
    {
      Key = "Env_Summary",
      Value = JsonSerializer.Serialize(new
      {
        cpuInfo.TotalCpuPercent,
        _options.CpuThresholdPercent,
        MemoryPercent = Math.Round(memPercent, 2),
        _options.MemoryThresholdPercent,
        TotalAvailableMemoryGB = SafeToGB(totalAvailableBytes),
        ApproxProcessesWorkingSetGB = SafeToGB(totalWorkingSet),
        _options.DiskThresholdPercent,
        _options.FailOnDiskThreshold
      }),
      Type = HealthCheckDefaults.ExtraInfoTypes.Json,
      Severity = status == HealthCheckStatus.OK ? ReportInformationSeverity.Info : ReportInformationSeverity.Error
    });

    extra.Add(new ReportExtraInformationEto
    {
      Key = "Env_Cpu",
      Value = JsonSerializer.Serialize(new { cpuInfo.TotalCpuPercent, Top5CpuProcesses = topCpu }),
      Type = HealthCheckDefaults.ExtraInfoTypes.Json,
      Severity = failCpu ? ReportInformationSeverity.Error : ReportInformationSeverity.Info
    });

    extra.Add(new ReportExtraInformationEto
    {
      Key = "Env_Memory",
      Value = JsonSerializer.Serialize(new { MemoryPercent = Math.Round(memPercent, 2), Top5MemoryProcesses = topMem }),
      Type = HealthCheckDefaults.ExtraInfoTypes.Json,
      Severity = failMem ? ReportInformationSeverity.Error : ReportInformationSeverity.Info
    });

    extra.Add(new ReportExtraInformationEto
    {
      Key = "Env_Disks",
      Value = JsonSerializer.Serialize(drives),
      Type = HealthCheckDefaults.ExtraInfoTypes.Json,
      Severity = failDisk ? ReportInformationSeverity.Error : ReportInformationSeverity.Info
    });

    report.Status = status;
    report.Message = message;
    report.ExtraInformation.AddRange(extra);
  }

  // -------- Helpers --------

  private static IEnumerable<Process> SafeGetProcesses()
  {
    try { return Process.GetProcesses(); }
    catch { return Enumerable.Empty<Process>(); }
  }
  private static string SafeName(Process p) { try { return p.ProcessName; } catch { return $"pid:{p.Id}"; } }
  private static int SafePid(Process p) { try { return p.Id; } catch { return -1; } }
  private static long SafeWS(Process p) { try { return p.WorkingSet64; } catch { return 0; } }
  private static double SafeToGB(long bytes) => Math.Round(bytes / 1024.0 / 1024.0 / 1024.0, 2);
  private static string SafeStr(Func<string> get) { try { return get(); } catch { return ""; } }

  private static async Task<CpuSample> SampleCpuAsync(int sampleMs)
  {
    var cores = Math.Max(1, global::System.Environment.ProcessorCount);
    var first = SafeGetProcesses().ToDictionary(SafePid, p => SafeCPU(p));
    await Task.Delay(sampleMs);
    var second = SafeGetProcesses().ToDictionary(SafePid, p => SafeCPU(p));

    var perProc = new List<PerProcessCpu>();
    double totalDeltaMs = 0;

    foreach (var kv in second)
    {
      if (!first.TryGetValue(kv.Key, out var prev)) continue;
      var delta = kv.Value.TotalProcessorTimeMs - prev.TotalProcessorTimeMs;
      if (delta < 0) delta = 0;

      totalDeltaMs += delta;
      var cpuPercent = (delta / (sampleMs * cores)) * 100.0;
      perProc.Add(new PerProcessCpu { Pid = kv.Key, Name = kv.Value.Name, CpuPercent = cpuPercent, WorkingSetBytes = kv.Value.WorkingSetBytes });
    }

    var totalCpu = (totalDeltaMs / (sampleMs * cores)) * 100.0;
    return new CpuSample { TotalCpuPercent = totalCpu, PerProcess = perProc };
  }

  private static ProcCpu SafeCPU(Process p)
  {
    try
    {
      return new ProcCpu
      {
        Name = SafeName(p),
        TotalProcessorTimeMs = p.TotalProcessorTime.TotalMilliseconds,
        WorkingSetBytes = SafeWS(p)
      };
    }
    catch { return new ProcCpu { Name = "unknown" }; }
  }

  private sealed class ProcCpu { public string Name { get; set; } = ""; public double TotalProcessorTimeMs { get; set; } public long WorkingSetBytes { get; set; } }
  private sealed class PerProcessCpu { public int Pid { get; set; } public string Name { get; set; } public double CpuPercent { get; set; } public long WorkingSetBytes { get; set; } }
  private sealed class CpuSample { public double TotalCpuPercent { get; set; } public List<PerProcessCpu> PerProcess { get; set; } = new(); }
}
