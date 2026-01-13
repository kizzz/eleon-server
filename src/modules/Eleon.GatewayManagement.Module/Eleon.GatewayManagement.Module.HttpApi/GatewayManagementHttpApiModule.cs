using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using GatewayManagement.Module.Proxies;
using System.Net.Http;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Http.Client;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.GatewayManagement.Module.Localization;

namespace VPortal.GatewayManagement.Module;

[DependsOn(
    typeof(GatewayManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class GatewayManagementHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(GatewayManagementHttpApiModule).Assembly);
    });

    PreConfigure<AbpHttpClientBuilderOptions>(options =>
    {
      options.ProxyClientHandlerActions.Add((remoteServiceName, services, httpClientHandler) =>
          {
          httpClientHandler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
          httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

          var currentGateway = services.GetRequiredService<CurrentGateway>();
          var gatewayCert = currentGateway.Options?.Certificate;
          if (gatewayCert != null)
          {
            httpClientHandler.ClientCertificates.Add(gatewayCert);
          }
        });

      options.AddGatewayHttpForwardHandler();
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<GatewayManagementResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });

    Configure<AbpAspNetCoreMvcOptions>(options =>
    {
      options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(GatewayForwardedResponseDto));
    });
  }
}
