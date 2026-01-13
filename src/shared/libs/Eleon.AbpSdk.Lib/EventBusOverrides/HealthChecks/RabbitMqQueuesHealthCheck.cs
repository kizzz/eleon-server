using Eleon.Common.Lib.modules.HealthCheck.Module.General;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckEventBus;

// -------- Options (configurable) --------
public sealed class RabbitManagementOptions
{
  /// <summary>Base URL with /api, e.g. http://localhost:15672/api</summary>
  public string BaseUrl { get; set; } = "http://localhost:15672/api";
  public string Username { get; set; } = "guest";
  public string Password { get; set; } = "guest";
  /// <summary>Optional vhost; if null/empty -> query all vhosts current user can see.</summary>
  public string? VirtualHost { get; set; }
  /// <summary>HTTP timeout (seconds).</summary>
  public int TimeoutSeconds { get; set; } = 30;

  public int ReadyThreshold { get; set; } = 200;
  public int UnackedThreshold { get; set; } = 100;
}

public sealed class RabbitMqQueuesHealthCheck : DefaultHealthCheck
{
  public override string Name => "RabbitMqQueues";
  public override bool IsPublic => true;

  private readonly RabbitManagementOptions _opt;

  public RabbitMqQueuesHealthCheck(IOptions<RabbitManagementOptions> options, IServiceProvider serviceProvider) : base(serviceProvider)
      => _opt = options.Value;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    try
    {
      var baseUrl = _opt.BaseUrl.TrimEnd('/');
      var url = (string.IsNullOrWhiteSpace(_opt.VirtualHost) || _opt.VirtualHost == "/")
          ? $"{baseUrl}/queues"
          : $"{baseUrl}/queues/{Uri.EscapeDataString(_opt.VirtualHost)}";

      AddJson(report, "Backend", new { Type = "RabbitMQ", _opt.BaseUrl, _opt.VirtualHost });

      using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(Math.Max(3, _opt.TimeoutSeconds)) };
      var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_opt.Username}:{_opt.Password}"));
      http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

      var json = await http.GetStringAsync(url);
      var queues = JsonSerializer.Deserialize<List<RabbitQueueDto>>(json, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      }) ?? new();

      var exceeded = queues
          .Where(q => q.messages_ready > _opt.ReadyThreshold || q.messages_unacknowledged > _opt.UnackedThreshold)
          .OrderByDescending(q => q.messages_ready)
          .ThenByDescending(q => q.messages_unacknowledged)
          .Select(q => new
          {
            q.vhost,
            q.name,
            Ready = q.messages_ready,
            Unacked = q.messages_unacknowledged,
            Total = q.messages,
            q.consumers
          })
          .ToList();

      AddJson(report, "Queues_ExceededThresholds", new
      {
        ReadyThreshold = _opt.ReadyThreshold,
        UnackedThreshold = _opt.UnackedThreshold,
        Count = exceeded.Count,
        Items = exceeded
      }, exceeded.Count > 0 ? ReportInformationSeverity.Error : ReportInformationSeverity.Info);

      AddSimple(report, "ObservedQueues", queues.Count.ToString());
      AddSimple(report, "QueriedAtUtc", DateTime.UtcNow.ToString("o"));

      if (exceeded.Count > 0)
      {
        report.Status = HealthCheckStatus.Failed;
        report.Message = "Some queues exceed backlog thresholds.";
      }
      else
      {
        report.Status = HealthCheckStatus.OK;
        report.Message = "All queues are within backlog thresholds.";
      }
    }
    catch (Exception ex)
    {
      report.Status = HealthCheckStatus.Failed;
      report.Message = $"Failed to query RabbitMQ management API: {ex.Message}";
      AddJson(report, "Exception", new { ex.GetType().Name, ex.Message, ex.StackTrace }, ReportInformationSeverity.Error);
    }
  }

  // --- DTO + helpers ---
  private sealed class RabbitQueueDto
  {
    public string? name { get; set; } = default!;
    public string? vhost { get; set; } = default!;
    public long messages { get; set; }
    public long messages_ready { get; set; }
    public long messages_unacknowledged { get; set; }
    public int consumers { get; set; }
  }

  private static void AddSimple(HealthCheckReportEto r, string key, string value,
      ReportInformationSeverity sev = ReportInformationSeverity.Info)
  {
    r.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = value,
      Severity = sev,
      Type = HealthCheckDefaults.ExtraInfoTypes.Simple
    });
  }

  private static void AddJson(HealthCheckReportEto r, string key, object value,
      ReportInformationSeverity sev = ReportInformationSeverity.Info)
  {
    r.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = JsonSerializer.Serialize(value),
      Severity = sev,
      Type = HealthCheckDefaults.ExtraInfoTypes.Json
    });
  }
}
