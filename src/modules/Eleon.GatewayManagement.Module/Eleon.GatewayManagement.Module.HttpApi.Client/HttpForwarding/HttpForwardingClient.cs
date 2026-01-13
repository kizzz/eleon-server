using Common.Module.Serialization;
using Logging.Module;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using GatewayManagement.Module.Proxies;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Content;
using Volo.Abp.DependencyInjection;
using VPortal.Identity.Module.MachineKeyValidation;

namespace GatewayManagement.Module.HttpForwarding
{
  public class HttpForwardingClient : ISingletonDependency
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IVportalLogger<HttpForwardingClient> logger;
    private readonly IGatewayHttpForwardingAppService httpForwardingAppService;
    private readonly IHttpForwardingHubUrlProvider hubUrlProvider;
    private readonly IHttpMessageSerializer httpMessageSerializer;
    private readonly IHttpRequestLoopback httpRequestLoopback;
    private readonly CancellationTokenSource connectionCts = new();
    private HubConnection connection = null;
    private AccessTokenProvider tokenProvider = new();
    private bool enabled = false;

    public HttpForwardingClient(
        IServiceProvider serviceProvider,
        IVportalLogger<HttpForwardingClient> logger,
        IGatewayHttpForwardingAppService httpForwardingAppService,
        IHttpForwardingHubUrlProvider hubUrlProvider,
        IHttpMessageSerializer httpMessageSerializer,
        IHttpRequestLoopback httpRequestLoopback)
    {
      this.serviceProvider = serviceProvider;
      this.logger = logger;
      this.httpForwardingAppService = httpForwardingAppService;
      this.hubUrlProvider = hubUrlProvider;
      this.httpMessageSerializer = httpMessageSerializer;
      this.httpRequestLoopback = httpRequestLoopback;
    }

    public async Task EnsureConnected(string accessToken)
    {
      if (!enabled)
      {
        return;
      }

      try
      {
        tokenProvider.AccessToken = accessToken;

        if (connection == null)
        {
          connection = CreateConnection();
        }

        if (connection.State == HubConnectionState.Disconnected)
        {
          await connection.StartAsync(connectionCts.Token);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public void Enable()
    {
      enabled = true;
    }

    public void Stop()
    {
      if (!enabled)
      {
        return;
      }

      try
      {
        if (connection != null)
        {
          connectionCts.Cancel();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private HubConnection CreateConnection()
    {
      string url = hubUrlProvider.GetHttpForwardingHubUrl();
      var connection = new HubConnectionBuilder()
          .WithUrl(new Uri(url), opt =>
          {
            opt.AccessTokenProvider = () => Task.FromResult(tokenProvider.AccessToken);
            opt.HttpMessageHandlerFactory = h =>
                  {
                var wrapper = serviceProvider.GetRequiredService<AppendMachineKeyHttpHandler>();
                wrapper.InnerHandler = h;
                return wrapper;
              };
          })
          .WithAutomaticReconnect()
          .Build();

      connection.On(
          HttpForwardingRemoteConsts.ForwardHttpRequestMethodName,
          async (string requestId) => await ForwardRequestToLoopback(Guid.Parse(requestId)));

      return connection;
    }

    private async Task ForwardRequestToLoopback(Guid requestId)
    {
      try
      {
        var requestStream = await httpForwardingAppService.GetForwardedRequest(requestId);
        var reader = new StreamReader(requestStream.GetStream(), Encoding.UTF8);
        var requestBase64 = await reader.ReadToEndAsync();
        var requestMsg = await httpMessageSerializer.CreateHttpRequestMessageFromBase64(requestBase64);

        var responseMsg = await httpRequestLoopback.SendRequest(requestMsg);
        string responseMsgBase64 = await httpMessageSerializer.HttpMessageToBase64(responseMsg);

        var forwardedResponse = new GatewayForwardedResponseDto()
        {
          ResponseId = requestId,
          ResponseContent = new RemoteStreamContent(new MemoryStream(responseMsgBase64.GetBytes())),
        };

        await httpForwardingAppService.SendForwardedResponse(forwardedResponse);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private class AccessTokenProvider
    {
      public string AccessToken { get; set; }
    }
  }
}
