using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
public class ConfigurationHealthCheck : DefaultHealthCheck
{

  public ConfigurationHealthCheck(CheckConfigurationService checkConfigurationService, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    CheckConfigurationService = checkConfigurationService;
  }

  public override string Name => "CheckConfiguration";

  public override bool IsPublic => true;

  public CheckConfigurationService CheckConfigurationService { get; }

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    var result = await CheckConfigurationService.CheckAsync();

    report.Status = result.Key ? HealthCheckStatus.OK : HealthCheckStatus.Failed;
    report.Message = result.Key ? "Configuration valid" : "Configuration invalid";
    report.ExtraInformation.AddRange(result.Value.Select(x => new ReportExtraInformationEto
    {
      Key = x.Key,
      Value = JsonSerializer.Serialize(x.Value),
      Severity = x.Value.IsErrored ? ReportInformationSeverity.Error : ReportInformationSeverity.Info,
      Type = HealthCheckDefaults.ExtraInfoTypes.Json
    }));
  }
}
