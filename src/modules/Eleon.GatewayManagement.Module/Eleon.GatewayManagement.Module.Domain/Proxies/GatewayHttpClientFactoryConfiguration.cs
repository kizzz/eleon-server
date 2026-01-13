using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace GatewayManagement.Module.Proxies
{
  internal class GatewayHttpClientFactoryConfiguration : IConfigureNamedOptions<HttpClientFactoryOptions>
  {
    public void Configure(string httpClientName, HttpClientFactoryOptions options)
    {
      Configure(options);
    }

    public void Configure(HttpClientFactoryOptions options)
    {
      options.HttpMessageHandlerBuilderActions.Add(builder =>
      {
        if (builder.PrimaryHandler is HttpClientHandler handler)
        {
          var currentGateway = builder.Services.GetRequiredService<CurrentGateway>();
          var gatewayCert = currentGateway.Options?.Certificate;
          if (gatewayCert != null)
          {
            handler.ClientCertificates.Add(gatewayCert);
          }
        }
      });
    }
  }
}
