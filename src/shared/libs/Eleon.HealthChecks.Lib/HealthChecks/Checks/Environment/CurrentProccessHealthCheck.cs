using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
public class CurrentProccessHealthCheck : DefaultHealthCheck
{
  private readonly CurrentProcessHealthCheckOptions _options;

  public override string Name => "CurrentProcess";
  public override bool IsPublic => true;

  public CurrentProccessHealthCheck(IOptions<CurrentProcessHealthCheckOptions> options, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    _options = options.Value;
  }

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    var extra = new List<ReportExtraInformationEto>();
    var proc = Process.GetCurrentProcess();

    // CPU (sample via TotalProcessorTime delta)
    var cpu = await SampleCurrentProcessCpuAsync(proc, _options.CpuSampleMilliseconds);
    var cpuFail = cpu > _options.CpuThresholdPercent;

    // Memory / GC
    proc.Refresh();
    double wsMB = BytesToMB(Safe(() => proc.WorkingSet64));
    double privateMB = BytesToMB(Safe(() => proc.PrivateMemorySize64));
    var gci = GC.GetGCMemoryInfo();
    double heapMB = BytesToMB(gci.HeapSizeBytes);
    double fragmentedMB = BytesToMB(gci.FragmentedBytes);
    var totalAllocatedMB = BytesToMB(GC.GetTotalAllocatedBytes(precise: false));
    var memFail = wsMB > _options.WorkingSetThresholdMB || heapMB > _options.HeapThresholdMB;

    // Threads / Handles / Uptime
    var threadCount = Safe(() => proc.Threads?.Count) ?? 0;
    var threadFail = threadCount > _options.ThreadThreshold;
    var handleCount = Safe(() => proc.HandleCount);
    var startTime = Safe(() => proc.StartTime);
    var uptime = startTime != default ? (DateTime.Now - startTime) : (TimeSpan?)null;

    // ThreadPool snapshot
    ThreadPool.GetAvailableThreads(out var availWorker, out var availIO);
    ThreadPool.GetMaxThreads(out var maxWorker, out var maxIO);
    ThreadPool.GetMinThreads(out var minWorker, out var minIO);
    var busyWorker = maxWorker - availWorker;
    var busyIO = maxIO - availIO;

    // Runtime / OS
    var frameworkDesc = RuntimeInformation.FrameworkDescription;
    var rid = RuntimeInformation.RuntimeIdentifier;
    var osDesc = RuntimeInformation.OSDescription;
    var osArch = RuntimeInformation.OSArchitecture.ToString();
    var procArch = RuntimeInformation.ProcessArchitecture.ToString();
    var envVer = global::System.Environment.Version.ToString();
    var latency = GCSettings.LatencyMode.ToString();
    var logicalProcs = global::System.Environment.ProcessorCount;

    // Process basics
    var pid = proc.Id;
    var name = Safe(() => proc.ProcessName);
    var basePriority = Safe(() => proc.BasePriority);
    var priorityClass = Safe(() => proc.PriorityClass.ToString());
    var totalCpuTime = Safe(() => proc.TotalProcessorTime.ToString());
    var userCpuTime = Safe(() => proc.UserProcessorTime.ToString());
    var kernelCpuTime = Safe(() => proc.PrivilegedProcessorTime.ToString());
    var machineName = Safe(() => proc.MachineName);
    var mainModulePath = Safe(() => proc.MainModule?.FileName);

    // GC Collection counts
    var gen0 = GC.CollectionCount(0);
    var gen1 = GC.CollectionCount(1);
    var gen2 = GC.CollectionCount(2);

    // Build status/message
    var failedReasons = new List<string>();
    if (cpuFail) failedReasons.Add($"CPU>{_options.CpuThresholdPercent}%");
    if (memFail) failedReasons.Add($"Mem>WS {_options.WorkingSetThresholdMB}MB or Heap>{_options.HeapThresholdMB}MB");
    if (threadFail) failedReasons.Add($"Threads>{_options.ThreadThreshold}");

    var status = failedReasons.Any() ? HealthCheckStatus.Failed : HealthCheckStatus.OK;
    var message = failedReasons.Any()
        ? $"Current process limits exceeded: {string.Join(", ", failedReasons)}."
        : "Current process is within thresholds.";

    // === Small, single-value entries (no bulky JSON) ===

    // Summary 
    Add(extra, "Proc.TimestampUtc", DateTime.UtcNow.ToString("O"), status == HealthCheckStatus.OK);
    Add(extra, "Proc.LogicalProcessors", logicalProcs.ToString(), status == HealthCheckStatus.OK);

    // CPU
    Add(extra, "Proc.CpuPercent", Round2(cpu).ToString(), !cpuFail);
    Add(extra, "Proc.CpuThresholdPercent", _options.CpuThresholdPercent.ToString(), true);
    Add(extra, "Proc.CpuSampleMs", _options.CpuSampleMilliseconds.ToString(), true);

    // Memory
    Add(extra, "Proc.WorkingSetMB", Round2(wsMB).ToString(), !memFail);
    Add(extra, "Proc.PrivateBytesMB", Round2(privateMB).ToString(), !memFail);
    Add(extra, "Proc.HeapMB", Round2(heapMB).ToString(), !memFail);
    Add(extra, "Proc.FragmentedMB", Round2(fragmentedMB).ToString(), !memFail);
    Add(extra, "Proc.TotalAllocatedMB", Round2(totalAllocatedMB).ToString(), !memFail);
    Add(extra, "Proc.WorkingSetThresholdMB", _options.WorkingSetThresholdMB.ToString(), true);
    Add(extra, "Proc.HeapThresholdMB", _options.HeapThresholdMB.ToString(), true);

    // Threads/Handles/Uptime
    Add(extra, "Proc.Threads", threadCount.ToString(), !threadFail);
    Add(extra, "Proc.ThreadThreshold", _options.ThreadThreshold.ToString(), true);
    Add(extra, "Proc.Handles", handleCount.ToString(), true);
    Add(extra, "Proc.StartTimeLocal", startTime.ToString("O"), true);
    if (uptime.HasValue) Add(extra, "Proc.Uptime", uptime.Value.ToString(), true);

    // ThreadPool
    Add(extra, "Proc.ThreadPool.MaxWorker", maxWorker.ToString(), true);
    Add(extra, "Proc.ThreadPool.MinWorker", minWorker.ToString(), true);
    Add(extra, "Proc.ThreadPool.AvailableWorker", availWorker.ToString(), true);
    Add(extra, "Proc.ThreadPool.BusyWorker", busyWorker.ToString(), true);
    Add(extra, "Proc.ThreadPool.MaxIO", maxIO.ToString(), true);
    Add(extra, "Proc.ThreadPool.MinIO", minIO.ToString(), true);
    Add(extra, "Proc.ThreadPool.AvailableIO", availIO.ToString(), true);
    Add(extra, "Proc.ThreadPool.BusyIO", busyIO.ToString(), true);

    // GC details
    Add(extra, "Proc.GC.Compacted", gci.Compacted.ToString(), !memFail);
    Add(extra, "Proc.GC.Concurrent", gci.Concurrent.ToString(), !memFail);
    Add(extra, "Proc.GC.PauseTimePct", gci.PauseTimePercentage.ToString("F2"), !memFail);
    Add(extra, "Proc.GC.Index", gci.Index.ToString(), !memFail);
    Add(extra, "Proc.GC.Gen0", gen0.ToString(), !memFail);
    Add(extra, "Proc.GC.Gen1", gen1.ToString(), !memFail);
    Add(extra, "Proc.GC.Gen2", gen2.ToString(), !memFail);
    Add(extra, "Proc.GC.LatencyMode", GCSettings.LatencyMode.ToString(), !memFail);

    // Runtime / OS
    Add(extra, "Proc.Runtime.Framework", frameworkDesc, true);
    Add(extra, "Proc.Runtime.RID", rid, true);
    Add(extra, "Proc.Runtime.OS", osDesc, true);
    Add(extra, "Proc.Runtime.OSArchitecture", osArch, true);
    Add(extra, "Proc.Runtime.ProcessArchitecture", procArch, true);
    Add(extra, "Proc.Runtime.EnvVersion", envVer, true);

    // Process details (basic)
    Add(extra, "Proc.Id", pid.ToString(), true);
    Add(extra, "Proc.Name", name ?? "", true);
    Add(extra, "Proc.BasePriority", basePriority.ToString() ?? "", true);
    Add(extra, "Proc.PriorityClass", priorityClass ?? "", true);
    Add(extra, "Proc.TotalProcessorTime", totalCpuTime ?? "", true);
    Add(extra, "Proc.UserProcessorTime", userCpuTime ?? "", true);
    Add(extra, "Proc.PrivilegedProcessorTime", kernelCpuTime ?? "", true);
    Add(extra, "Proc.MachineName", machineName ?? "", true);
    Add(extra, "Proc.MainModulePath", mainModulePath ?? "", true);

    report.Status = status;
    report.Message = message;
    report.ExtraInformation.AddRange(extra);
  }

  // ---- helpers ----

  private static double BytesToMB(long bytes) => Math.Round(bytes / 1024.0 / 1024.0, 2);
  private static double Round2(double v) => Math.Round(v, 2);

  private static T? Safe<T>(Func<T> fn)
  {
    try { return fn(); } catch { return default; }
  }

  private static void Add(List<ReportExtraInformationEto> extra, string key, string value, bool okSeverity)
  {
    extra.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = value ?? "",
      Type = HealthCheckDefaults.ExtraInfoTypes.Simple,
      Severity = okSeverity ? ReportInformationSeverity.Info : ReportInformationSeverity.Error
    });
  }

  private static async Task<double> SampleCurrentProcessCpuAsync(Process p, int sampleMs)
  {
    var cores = Math.Max(1, global::System.Environment.ProcessorCount);
    var t1 = Safe(() => p.TotalProcessorTime.TotalMilliseconds);
    var sw = Stopwatch.StartNew();
    await Task.Delay(sampleMs);
    var elapsed = Math.Max(1, sw.ElapsedMilliseconds);
    var t2 = Safe(() => p.TotalProcessorTime.TotalMilliseconds);

    var delta = Math.Max(0.0, t2 - t1);
    var cpu = (delta / (elapsed * cores)) * 100.0;
    return Math.Min(100.0, Math.Max(0.0, cpu));
  }
}
