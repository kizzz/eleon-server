using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Keys;
using GatewayManagement.Module.Repositories;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace VPortal.GatewayManagement.Module.DomainServices
{

  public class GatewayRegistrationManager : ITransientDependency
  {
    private readonly IVportalLogger<GatewayRegistrationManager> logger;
    private readonly IGatewayRepository gatewayRepository;
    private readonly IGatewayPrivateDetailsRepository gatewayPrivateDetailsRepository;
    private readonly ICurrentTenant currentTenant;
    private readonly IGuidGenerator guidGenerator;
    private readonly IDistributedEventBus eventBus;

    public GatewayRegistrationManager(
        IVportalLogger<GatewayRegistrationManager> logger,
        IGatewayRepository gatewayRepository,
        IGatewayPrivateDetailsRepository gatewayPrivateDetailsRepository,
        ICurrentTenant currentTenant,
        IGuidGenerator guidGenerator,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.gatewayRepository = gatewayRepository;
      this.gatewayPrivateDetailsRepository = gatewayPrivateDetailsRepository;
      this.currentTenant = currentTenant;
      this.guidGenerator = guidGenerator;
      this.eventBus = eventBus;
    }

    public async Task ActivateGateway(Guid gatewayId)
    {
      try
      {
        var gatewayDetails = await gatewayPrivateDetailsRepository.GetByGateway(gatewayId);
        await RegisterApiKeyInIdentity(gatewayId, gatewayDetails.ClientKey, gatewayDetails.MachineKey);

        var gateway = await gatewayRepository.GetAsync(gatewayId);

        var vpnPeer = await RegisterVpnPeer(gatewayId);
        gateway.VpnAdapterName = "EleonsoftVPN";
        gateway.VpnAdapterGuid = guidGenerator.Create();
        gateway.VpnAddress = vpnPeer.PeerEto.AllowedIps.Split('/').First();
        gateway.VpnPublicKey = vpnPeer.PeerEto.PublicKey;
        gateway.VpnPrivateKey = vpnPeer.PeerEto.PrivateKey;

        gateway.Status = GatewayStatus.WaitingForConfirmation;
        await gatewayRepository.UpdateAsync(gateway, true);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task<VpnPeerAddedMsg> RegisterVpnPeer(Guid gatewayId)
    {
      var msg = new AddVpnPeerMsg()
      {
        RefId = gatewayId.ToString(),
      };

      var response = await eventBus.RequestAsync<VpnPeerAddedMsg>(msg);
      if (response.PeerEto == null)
      {
        throw new Exception("Could not register API key in Identity.");
      }

      return response;
    }

    private async Task RegisterApiKeyInIdentity(Guid gatewayId, string clientKey, string machineKey)
    {
      var msg = new AddApiKeyMsg
      {
        ApiKey = new ClientMachineCompoundKey(clientKey, machineKey, currentTenant.Id).ToString(),
        Subject = gatewayId.ToString(),
        Type = ApiKeyType.Gateway,
        AllowAuthorize = true,
      };
      var response = await eventBus.RequestAsync<ApiKeyAddedMsg>(msg);
      bool success = response.AddedSuccessfully;
      if (!success)
      {
        throw new Exception("Could not register API key in Identity.");
      }
    }
  }
}
