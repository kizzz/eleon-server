using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Helpers;
using Logging.Module;
using Messaging.Module.Messages;
using GatewayManagement.Module.Entities;
using GatewayManagement.Module.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.GatewayManagement.Module;
using System.Security.Cryptography;
using System.Text;
using VPortal.GatewayManagement.Module.DomainServices;
using EventBusManagement.Module.EntityFrameworkCore;
using EventBusManagement.Module;

namespace GatewayManagement.Module.Proxies
{

  public class GatewayManagementDomainService : DomainService
  {
    private readonly IGatewayRepository gatewayRepository;
    private readonly IGatewayPrivateDetailsRepository gatewayDetailsRepository;
    private readonly IGatewayRegistrationKeysRepository gatewayRegistrationKeysRepository;
    private readonly ICurrentUser currentUser;
    private readonly GatewayStaticKeyDomainService gatewayStaticKeyDomainService;
    private readonly GatewayRegistrationManager gatewayRegistrationManager;
    private readonly EventBusManager eventBusManager;
    private readonly IVportalLogger<GatewayManagementDomainService> logger;
    private readonly IDistributedEventBus eventBus;

    public GatewayManagementDomainService(
        IGatewayRepository gatewayRepository,
        IGatewayPrivateDetailsRepository gatewayDetailsRepository,
        IGatewayRegistrationKeysRepository gatewayRegistrationKeysRepository,
        ICurrentUser currentUser,
        GatewayStaticKeyDomainService gatewayStaticKeyDomainService,
        GatewayRegistrationManager gatewayRegistrationManager,
        EventBusManager eventBusManager,
        IVportalLogger<GatewayManagementDomainService> logger,
        IDistributedEventBus eventBus)
    {
      this.gatewayRepository = gatewayRepository;
      this.gatewayDetailsRepository = gatewayDetailsRepository;
      this.gatewayRegistrationKeysRepository = gatewayRegistrationKeysRepository;
      this.currentUser = currentUser;
      this.gatewayStaticKeyDomainService = gatewayStaticKeyDomainService;
      this.gatewayRegistrationManager = gatewayRegistrationManager;
      this.eventBusManager = eventBusManager;
      this.logger = logger;
      this.eventBus = eventBus;
    }

    public async Task<GatewayEntity> GetGateway(Guid gatewayId)
    {
      GatewayEntity result = null;
      try
      {
        result = await gatewayRepository.GetAsync(gatewayId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> UpdateGateway(GatewayEntity gateway, EventBusEntity eventBus, bool useDefaultEventBus)
    {
      bool result = false;
      try
      {
        var trackedEntity = await gatewayRepository.GetAsync(gateway.Id);
        trackedEntity.Name = gateway.Name;

        if (useDefaultEventBus)
        {
          var defaultBus = await eventBusManager.GetDefaultEventBus();
          trackedEntity.EventBusId = defaultBus.Id;
        }
        else if (gateway.SelfHostEventBus && trackedEntity.EventBusId == null)
        {
          var busId = GuidGenerator.Create();
          var newBus = new EventBusEntity(busId, eventBus.Provider, eventBus.ProviderOptions, EventBusStatus.Initialization)
          {
            Name = gateway.Name + " Bus",
          };

          await eventBusManager.AddEventBus(newBus);
          trackedEntity.EventBusId = busId;
        }
        else if (gateway.SelfHostEventBus && trackedEntity.EventBusId != null)
        {
          await eventBusManager.UpdateEventBus(eventBus);
        }
        else
        {
          trackedEntity.EventBusId = gateway.EventBusId;
        }

        await gatewayRepository.UpdateAsync(trackedEntity);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<GatewayRegistrationKeyEntity> RequestGatewayRegistration(Guid gatewayId)
    {
      GatewayRegistrationKeyEntity result = null;
      try
      {
        var gateway = await gatewayRepository.GetAsync(gatewayId);

        var details = await gatewayDetailsRepository.GetByGateway(gatewayId);
        details.ClientKey = GenerateClientKey();
        details.MachineKey = null;
        await gatewayDetailsRepository.UpdateAsync(details, true);

        await InvalidateAllKeys(gatewayId);

        var newRegKey = new GatewayRegistrationKeyEntity(GuidGenerator.Create())
        {
          ExpirationDate = DateTime.UtcNow.AddMinutes(10),
          GatewayId = gatewayId,
          Status = GatewayRegistrationKeyStatus.NotUsed,
          Key = Encoding.UTF8.GetString(MD5.HashData(GuidGenerator.Create().ToString().GetBytes())),
        };
        await gatewayRegistrationKeysRepository.InsertAsync(newRegKey, true);

        gateway.Status = GatewayStatus.WaitingForRegistration;

        await gatewayRepository.UpdateAsync(gateway, true);

        result = newRegKey;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<Guid> AddPendingGateway()
    {
      Guid result = default;
      try
      {
        var gateway = new GatewayEntity(GuidGenerator.Create(), string.Empty, GatewayProtocol.HTTPS);
        gateway = await gatewayRepository.InsertAsync(gateway);

        var gatewayDetails = new GatewayPrivateDetailsEntity(GuidGenerator.Create())
        {
          GatewayId = gateway.Id,
          ClientKey = GenerateClientKey(),
        };
        await gatewayDetailsRepository.InsertAsync(gatewayDetails, true);

        gateway.Status = GatewayStatus.WaitingForAccept;

        await gatewayRepository.UpdateAsync(gateway, true);

        result = gateway.Id;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task RejectPendingGateway(Guid gatewayId)
    {
      try
      {
        var gateway = await gatewayRepository.GetAsync(gatewayId);

        if (gateway.Status != GatewayStatus.WaitingForAccept)
        {
          throw new Exception("Gateway is not waiting for accept/reject.");
        }

        gateway.Status = GatewayStatus.Rejected;

        await gatewayRepository.UpdateAsync(gateway, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task AcceptPendingGateway(Guid gatewayId, string name)
    {
      try
      {
        var gateway = await gatewayRepository.GetAsync(gatewayId);

        if (gateway.Status != GatewayStatus.WaitingForAccept)
        {
          throw new Exception("Gateway is not waiting for accept/reject.");
        }

        var privateDetails = await gatewayDetailsRepository.GetByGateway(gatewayId);

        await gatewayRegistrationManager.ActivateGateway((Guid)gateway.Id);

        gateway.Name = name;
        await gatewayRepository.UpdateAsync(gateway, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task<GatewayRegistrationKeyEntity> GetCurrentGatewayRegistrationKey(Guid gatewayId)
    {
      GatewayRegistrationKeyEntity result = null;
      try
      {
        var gateway = await gatewayRepository.GetAsync(gatewayId);
        result = await gatewayRegistrationKeysRepository.GetLastAdded(gatewayId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task CancelOngoingGatewayRegistration(Guid gatewayId)
    {
      try
      {
        var gateway = await gatewayRepository.GetAsync(gatewayId);
        bool isOngoing = gateway.Status != GatewayStatus.Active;
        if (!isOngoing)
        {
          throw new Exception("Can not cancel registration, as the gateway is already registered.");
        }

        await InvalidateAllKeys(gatewayId);

        gateway.Status = GatewayStatus.New;
        await gatewayRepository.UpdateAsync(gateway);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task<List<GatewayEntity>> GetGatewayList(GatewayStatus? status)
    {
      List<GatewayEntity> result = null;
      try
      {
        if (status == null)
        {
          result = await gatewayRepository.GetList();
        }
        else
        {
          result = await gatewayRepository.GetList(status);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<string> AddGateway(GatewayEntity gateway)
    {
      string result = null;
      try
      {
        gateway.Status = GatewayStatus.New;
        var saved = await gatewayRepository.InsertAsync(gateway);

        var gatewayDetails = new GatewayPrivateDetailsEntity(GuidGenerator.Create())
        {
          GatewayId = gateway.Id,
        };
        await gatewayDetailsRepository.InsertAsync(gatewayDetails);

        result = saved.Id.ToString();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task RemoveGateway(Guid gatewayId)
    {
      try
      {
        var gateway = await gatewayRepository.GetAsync(gatewayId);
        if (gateway.Status == GatewayStatus.Active)
        {
          //var unassignRequest = new UnassignGatewayMsg(null, null);
          //unassignRequest.GatewayId = gatewayId;
          //var unassignResponse = await unassignGatewayClient.RequestAsync<GatewayUnassignedMsg>(unassignRequest);
          //bool success = unassignResponse.Success;
          //bool unableToUnassign = unassignResponse.CanNotBeUnassigned;
          //if (!success && unableToUnassign)
          //{
          //    throw new BusinessException(GatewayManagementErrorCodes.CanNotDeleteAssignedGateway);
          //}
          //else if (!success)
          //{
          //    throw new Exception("An error occured while unassigning gateway.");
          //}
        }

        await gatewayRepository.DeleteAsync(gatewayId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    internal async Task<X509Certificate2> GetGatewayCertificate(Guid gatewayId)
    {
      X509Certificate2 result = null;
      try
      {
        var gatewayDetails = await gatewayDetailsRepository.GetByGateway(gatewayId);
        var certBytes = Convert.FromBase64String(gatewayDetails.CertificatePemBase64);
        var cert = CertificateHelper.CreateCertificateFromPem(certBytes, gatewayDetails.MachineKey);
        result = cert;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    /// <summary>
    /// Gets the private keys that are attached to a gateway.
    /// </summary>
    /// <param name="tenantId">The ID of tenant that will be used to make a query. It's required as the method can be called from outside the request scope.</param>
    /// <param name="gatewayId">The ID of the gateway.</param>
    /// <returns>The machine and the client keys of the requested gateway within the requested tenant, or null if the gateway was not found.</returns>
    internal async Task<(string machineKey, string clientKey)?> GetGatewayPrivateDetails(Guid? tenantId, Guid gatewayId)
    {
      (string machineKey, string clientKey)? result = null;
      try
      {
        using (CurrentTenant.Change(tenantId))
        {
          await EnsureGatewayIsActive(gatewayId);
          var gatewayDetails = await gatewayDetailsRepository.GetByGateway(gatewayId);
          return (gatewayDetails.MachineKey, gatewayDetails.ClientKey);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    private async Task InvalidateAllKeys(Guid gatewayId)
    {
      var existingKeys = await gatewayRegistrationKeysRepository.GetListByGateway(gatewayId);
      if (existingKeys.Any())
      {
        foreach (var reg in existingKeys)
        {
          if (reg.Status is not GatewayRegistrationKeyStatus.Succeeded)
          {
            reg.Status = GatewayRegistrationKeyStatus.Failed;
          }

          reg.Invalidated = true;
        }

        await gatewayRegistrationKeysRepository.UpdateManyAsync(existingKeys);
      }
    }

    private async Task EnsureGatewayIsActive(Guid gatewayId)
    {
      var gateway = await gatewayRepository.GetAsync(gatewayId);
      if (gateway.Status != GatewayStatus.Active)
      {
        throw new Exception("The gateway is not in the 'Active' status so its details are absent or not verified.");
      }
    }

    private string GenerateClientKey()
        => Convert.ToHexString(SHA512.HashData(GuidGenerator.Create().ToString().GetBytes()));
  }
}
