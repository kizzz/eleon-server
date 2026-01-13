using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using GatewayManagement.Module.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.GatewayManagement.Module;
using VPortal.GatewayManagement.Module.Permissions;
using VPortal.GatewayManagement.Module.Proxies;
using System.Linq;
using Common.Module.Constants;
using EventBusManagement.Module;
using System.Diagnostics.Eventing.Reader;
using Volo.Abp.EventBus.Distributed;
using Common.EventBus.Module;
using Messaging.Module.Messages;

namespace GatewayManagement.Module.Proxies
{
  [Authorize(GatewayManagementPermissions.Gateway)]
  public class GatewayClientAppService : GatewayManagementBaseAppService, IGatewayClientAppService
  {
    private readonly IVportalLogger<GatewayClientAppService> logger;
    private readonly IDistributedEventBus eventBus;
    private readonly EventBusManager eventBusManager;
    private readonly GatewayClientDomainService gatewayDomainService;

    public GatewayClientAppService(
        GatewayClientDomainService gatewayDomainService,
        IVportalLogger<GatewayClientAppService> logger,
        IDistributedEventBus eventBus,
        EventBusManager eventBusManager)
    {
      this.gatewayDomainService = gatewayDomainService;
      this.logger = logger;
      this.eventBus = eventBus;
      this.eventBusManager = eventBusManager;
    }

    public async Task<bool> ConfirmGatewayRegistration()
    {

      bool response = false;
      try
      {
        response = await gatewayDomainService.ConfirmGatewayRegistration();
      }
      catch (BusinessException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<GatewayDto> GetCurrentGateway()
    {

      GatewayDto response = null;
      try
      {
        var gatewayEntity = await gatewayDomainService.GetCurrentGateway();
        response = ObjectMapper.Map<GatewayEntity, GatewayDto>(gatewayEntity);
      }
      catch (BusinessException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<GatewayWorkspaceDto> GetCurrentGatewayWorkspace(string workspaceName)
    {

      GatewayWorkspaceDto response = null;
      try
      {
        var gateway = await gatewayDomainService.GetCurrentGateway();
        var bus = await eventBusManager.GetEventBus(gateway.EventBusId.Value);
        var vpnServerSettings = (await eventBus.RequestAsync<VpnSettingsGotMsg>(new GetVpnSettingsMsg())).Settings;

        response = ObjectMapper.Map<GatewayEntity, GatewayWorkspaceDto>(gateway);
        response.EventBusProvider = bus.Provider;
        response.EventBusProviderOptionsJson = bus.ProviderOptions;
        response.VpnServerAddress = vpnServerSettings.Address;
        response.VpnServerPublicKey = vpnServerSettings.PublicKey;
      }
      catch (BusinessException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task SetGatewayHealthStatus(SetGatewayHealthStatusRequestDto request)
    {
      try
      {
        await gatewayDomainService.SetGatewayHealthStatus(request.HealthStatus);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    [AllowAnonymous]
    public async Task<GatewayRegistrationResultDto> RegisterGateway(RegisterGatewayRequestDto request)
    {

      GatewayRegistrationResultDto response = null;
      try
      {
        byte[] certificate = Convert.FromBase64String(request.CertificateBase64);
        var result = await gatewayDomainService.RegisterGateway(request.RegistrationKey, request.MachineKey, certificate);
        response = new GatewayRegistrationResultDto()
        {
          Status = result.status,
          ClientKey = result.clientKey,
        };
      }
      catch (BusinessException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }
  }
}
