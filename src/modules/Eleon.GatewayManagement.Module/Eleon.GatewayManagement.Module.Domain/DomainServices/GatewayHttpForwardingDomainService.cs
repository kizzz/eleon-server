using Common.Module.Serialization;
using Logging.Module;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace GatewayManagement.Module.Proxies
{

  [ExposeServices(typeof(IGatewayHttpMessageForwarder), IncludeSelf = true)]
  public class GatewayHttpForwardingDomainService : DomainService, IGatewayHttpMessageForwarder
  {
    private static readonly TimeSpan ForwardingTimeout = TimeSpan.FromMinutes(3);
    private readonly IVportalLogger<GatewayHttpForwardingDomainService> logger;
    private readonly IHttpMessageSerializer httpMessageSerializer;
    private readonly IGatewayHttpForwardHubContext gatewayHttpForwardHubContext;
    private readonly GatewayForwardedRequestManager gatewayForwardedRequestManager;

    public GatewayHttpForwardingDomainService(
        IVportalLogger<GatewayHttpForwardingDomainService> logger,
        IHttpMessageSerializer httpMessageSerializer,
        IGatewayHttpForwardHubContext gatewayHttpForwardHubContext,
        GatewayForwardedRequestManager gatewayForwardedRequestManager)
    {
      this.logger = logger;
      this.httpMessageSerializer = httpMessageSerializer;
      this.gatewayHttpForwardHubContext = gatewayHttpForwardHubContext;
      this.gatewayForwardedRequestManager = gatewayForwardedRequestManager;
    }

    public async Task<HttpResponseMessage> ForwardHttpRequestMessageToGateway(Guid gatewayId, HttpRequestMessage request)
    {
      HttpResponseMessage response = null;
      try
      {
        var requestBase64 = await httpMessageSerializer.HttpMessageToBase64(request);
        Guid requestId = gatewayForwardedRequestManager.AddForwardedRequest(requestBase64);

        await gatewayHttpForwardHubContext.ForwardHttpRequestToGateway(gatewayId, requestId);

        var responseBase64 = await gatewayForwardedRequestManager.WaitForResponse(requestId, ForwardingTimeout);
        response = await httpMessageSerializer.CreateHttpResponseMessageFromBase64(responseBase64);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return response;
    }

    public async Task<string> GetForwardedRequest(Guid requestId)
    {
      string result = null;
      try
      {
        result = gatewayForwardedRequestManager.GetForwardedRequest(requestId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SendForwardedResponse(Guid requestId, string response)
    {
      try
      {
        gatewayForwardedRequestManager.SetForwardedRequestResponse(requestId, response);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
