using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
public class HttpHealthCheck : DefaultHealthCheck
{
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly HttpHealthCheckOptions _options;

  public const string DefaultHealthCheckClientName = "HealthCheckClient";

  public HttpHealthCheck(IHttpClientFactory httpClientFactory, IOptions<HttpHealthCheckOptions> options, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    _httpClientFactory = httpClientFactory;
    _options = options.Value;
  }

  public override string Name => "HttpCheck";

  public override bool IsPublic => true;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    // try to check if service has bean started
    // todo return task completed task and update result when done to not block service statup
    report.Status = HealthCheckStatus.OK;
    report.Message = _options.Urls.Count > 0 ? "All HTTP checks passed" : "No HTTP Checks defined";

    if (_options.IgnoreSsl)
    {
      report.ExtraInformation.Add(new ReportExtraInformationEto
      {
        Key = "IgnoreSsl",
        Value = "WARNING: SSL check was disabled",
        Severity = ReportInformationSeverity.Warning,
      });
    }

    foreach (var url in _options.Urls)
    {
      try
      {
        var stopwatch = Stopwatch.StartNew();
        var client = _httpClientFactory.CreateClient(DefaultHealthCheckClientName);
        var response = await client.GetAsync(url.Url);
        stopwatch.Stop();
        report.ExtraInformation.Add(new ReportExtraInformationEto { Key = $"StatusCode_{url.Name}", Value = response.StatusCode.ToString() });
        report.ExtraInformation.Add(new ReportExtraInformationEto { Key = $"ResponseTimeMs_{url.Name}", Value = stopwatch.ElapsedMilliseconds.ToString() });

        if (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength.Value < 1024)
        {
          report.ExtraInformation.Add(new ReportExtraInformationEto { Key = $"Content_{url.Name}", Value = await response.Content.ReadAsStringAsync() });
        }
        else
        {
          report.ExtraInformation.Add(new ReportExtraInformationEto { Key = $"Content_{url.Name}", Value = response.Content.Headers.ContentLength.HasValue ? $"Large content {response.Content.Headers.ContentLength.Value} bytes" : "unknown length" });
        }
        if (!url.GoodStatusCodes.Contains((int)response.StatusCode))
        {
          var failureMessage = $"HTTP check failed for {url.Name} {url.Url}: Bad status code {response.StatusCode}";
          if (report.Status != HealthCheckStatus.Failed)
          {
            report.Status = HealthCheckStatus.Failed;
            report.Message = failureMessage;
          }
          else
          {
            report.Message = $"{report.Message}; {failureMessage}";
          }
        }
      }
      catch (Exception ex)
      {
        var failureMessage = $"HTTP check failed for {url.Name} {url.Url}: {ex.Message}";
        if (report.Status != HealthCheckStatus.Failed)
        {
          report.Status = HealthCheckStatus.Failed;
          report.Message = failureMessage;
        }
        else
        {
          report.Message = $"{report.Message}; {failureMessage}";
        }

        report.ExtraInformation.Add(new ReportExtraInformationEto
        {
          Key = "StackTrace",
          Value = ex.StackTrace ?? "No stack trace",
          Severity = ReportInformationSeverity.Error
        });
      }
    }
  }
}
