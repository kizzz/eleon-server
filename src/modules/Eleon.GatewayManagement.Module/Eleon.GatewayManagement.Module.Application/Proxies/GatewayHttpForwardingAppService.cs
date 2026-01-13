using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Content;
using VPortal.GatewayManagement.Module;
using VPortal.GatewayManagement.Module.Permissions;

namespace GatewayManagement.Module.Proxies
{
  [Authorize(GatewayManagementPermissions.Gateway)]
  public class GatewayHttpForwardingAppService : GatewayManagementBaseAppService, IGatewayHttpForwardingAppService
  {
    private readonly IVportalLogger<GatewayHttpForwardingAppService> logger;
    private readonly GatewayHttpForwardingDomainService gatewayHttpForwardingDomainService;

    public GatewayHttpForwardingAppService(
        IVportalLogger<GatewayHttpForwardingAppService> logger,
        GatewayHttpForwardingDomainService gatewayHttpForwardingDomainService)
    {
      this.logger = logger;
      this.gatewayHttpForwardingDomainService = gatewayHttpForwardingDomainService;
    }

    public async Task<IRemoteStreamContent> GetForwardedRequest(Guid requestId)
    {
      IRemoteStreamContent stream = null;
      try
      {
        var forwardedRequest = await gatewayHttpForwardingDomainService.GetForwardedRequest(requestId);
        stream = new RemoteStreamContent(new MemoryStream(forwardedRequest.GetBytes()));
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return stream;
    }

    public async Task<bool> SendForwardedResponse(GatewayForwardedResponseDto forwardedResponse)
    {
      bool response = false;
      try
      {
        var remoteStream = forwardedResponse.ResponseContent.GetStream();
        //var decompressStream = new GZipStream(remoteStream, CompressionMode.Decompress);
        var reader = new StreamReader(remoteStream, Encoding.UTF8);
        var forwardedResponseContent = await reader.ReadToEndAsync();
        await gatewayHttpForwardingDomainService.SendForwardedResponse(forwardedResponse.ResponseId, forwardedResponseContent);
        response = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return response;
    }
  }
}
