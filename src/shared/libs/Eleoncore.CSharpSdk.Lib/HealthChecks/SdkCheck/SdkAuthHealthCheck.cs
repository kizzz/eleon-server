using Eleon.Common.Lib.modules.HealthCheck.Module.General;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftProxy.Api;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.HttpApi.Helpers;

namespace EleoncoreAspNetCoreSdk.HealthChecks.SdkCheck;
public class SdkAuthHealthCheck : DefaultHealthCheck
{
  private readonly ITenantApi _tenantApi;

  public SdkAuthHealthCheck(
      ITenantApi tenantApi, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    _tenantApi = tenantApi;
  }

  public override string Name => "SdkCheck";

  public override bool IsPublic => true;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    var status = HealthCheckStatus.OK;
    string message = "Sdk api works successfully";
    string apiAccessToken = null;
    try
    {
      var eleonsoftConfigurator = ApiConfigurator.GetConfigurator(nameof(EleonsoftProxy));
      apiAccessToken = await TokenHelperService.GetAccessToken("api", eleonsoftConfigurator.SdkConfig, null);
      if (string.IsNullOrWhiteSpace(apiAccessToken))
      {
        throw new Exception("Eleonsoft: API access token is null or empty");
      }
    }
    catch (Exception ex)
    {
      status = HealthCheckStatus.Failed;
      message = $"Exception while getting API access token: {ex.GetType().Name}: {ex.Message}";
    }

    try
    {
      _tenantApi.UseApiAuth();
      var result = await _tenantApi.CoreTenantGetCommonTenantListAsync();
      if (!result.IsSuccessStatusCode)
      {
        throw new Exception($"Failed to call Tenant API. Status code: {result.StatusCode}");
      }
    }
    catch (Exception ex)
    {
      if (status == HealthCheckStatus.Failed)
      {
        message += $" Failed to call api: {ex.GetType().Name}: {ex.Message}";
      }
      else
      {
        message = $"Failed to call api: {ex.GetType().Name}: {ex.Message}";
      }
      status = HealthCheckStatus.Failed;

    }

    report.Status = status;
    report.Message = message;
  }
}


public static class SdkHealthCheckExtensions
{
  public static IServiceCollection AddSdkHealthCheck(this IServiceCollection services)
  {
    services.AddTransient<IEleonsoftHealthCheck, SdkAuthHealthCheck>();
    return services;
  }
}
