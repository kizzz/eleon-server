using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.Keys;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using GatewayManagement.Module.Entities;
using GatewayManagement.Module.Repositories;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TenantSettings.Module.Helpers;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.GatewayManagement.Module;
using Volo.Abp.Security.Claims;
using System.Collections.Generic;
using Newtonsoft.Json;
using Common.EventBus.Module.Options;
using EventBusManagement.Module;
using VPortal.GatewayManagement.Module.DomainServices;
using System.Security.Cryptography;

namespace GatewayManagement.Module.Proxies
{

  public class GatewayClientDomainService : DomainService
  {
    private readonly IGatewayRepository gatewayRepository;
    private readonly IGatewayPrivateDetailsRepository gatewayDetailsRepository;
    private readonly IGatewayRegistrationKeysRepository gatewayRegistrationKeysRepository;
    private readonly IdentityUserManager userManager;
    private readonly ICurrentPrincipalAccessor currentPrincipalAccessor;
    private readonly IDistributedEventBus addIdentitySecretClient;
    private readonly MultiTenancyDomainService multiTenancyDomainService;
    private readonly EventBusConnector eventBusConnector;
    private readonly GatewayManagementDomainService gatewayManagementDomainService;
    private readonly IPermissionManager permissionManager;
    private readonly GatewayStaticKeyDomainService staticKeyDomainService;
    private readonly GatewayRegistrationManager gatewayRegistrationManager;
    private readonly IVportalLogger<GatewayClientDomainService> logger;

    public GatewayClientDomainService(
        IGatewayRepository gatewayRepository,
        IGatewayPrivateDetailsRepository gatewayDetailsRepository,
        IGatewayRegistrationKeysRepository gatewayRegistrationKeysRepository,
        IdentityUserManager userManager,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IDistributedEventBus addIdentitySecretClient,
        MultiTenancyDomainService multiTenancyDomainService,
        EventBusConnector eventBusConnector,
        GatewayManagementDomainService gatewayManagementDomainService,
        IPermissionManager permissionManager,
        GatewayStaticKeyDomainService staticKeyDomainService,
        GatewayRegistrationManager gatewayRegistrationManager,
        IVportalLogger<GatewayClientDomainService> logger)
    {
      this.gatewayRepository = gatewayRepository;
      this.gatewayDetailsRepository = gatewayDetailsRepository;
      this.gatewayRegistrationKeysRepository = gatewayRegistrationKeysRepository;
      this.userManager = userManager;
      this.currentPrincipalAccessor = currentPrincipalAccessor;
      this.addIdentitySecretClient = addIdentitySecretClient;
      this.multiTenancyDomainService = multiTenancyDomainService;
      this.eventBusConnector = eventBusConnector;
      this.gatewayManagementDomainService = gatewayManagementDomainService;
      this.permissionManager = permissionManager;
      this.staticKeyDomainService = staticKeyDomainService;
      this.gatewayRegistrationManager = gatewayRegistrationManager;
      this.logger = logger;
    }

    public async Task<(GatewayStatus status, string clientKey)> RegisterGateway(string registrationKey, string encryptedMachineKey, byte[] certificatePem)
    {
      (GatewayStatus status, string clientKey) result = default;
      try
      {
        bool isStaticKey = await staticKeyDomainService.IsValidStaticKey(registrationKey);
        string machineKey = EncryptionHelper.Decrypt(encryptedMachineKey);
        if (isStaticKey)
        {
          var gatewayId = await gatewayManagementDomainService.AddPendingGateway();

          string clientKey = await SetGatewayMachineDetails(gatewayId, machineKey, certificatePem);

          string compoundKey = new ClientCompoundKey(clientKey, null).ToString();
          string encryptedCompoundKey = EncryptionHelper.Encrypt(compoundKey);
          result = (GatewayStatus.WaitingForAccept, encryptedCompoundKey);
        }
        else
        {
          var keyEntity = await ClaimGatewayRegistrationKey(registrationKey);
          using (CurrentTenant.Change(keyEntity.TenantId))
          {
            string clientKey = await SetGatewayMachineDetails(keyEntity.GatewayId.Value, machineKey, certificatePem);

            var gateway = await gatewayRepository.GetAsync(keyEntity.GatewayId.Value);

            await gatewayRegistrationManager.ActivateGateway((Guid)gateway.Id);

            string compoundKey = new ClientCompoundKey(clientKey, keyEntity.TenantId).ToString();
            string encryptedCompoundKey = EncryptionHelper.Encrypt(compoundKey);
            result = (GatewayStatus.WaitingForConfirmation, encryptedCompoundKey);
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> ConfirmGatewayRegistration()
    {
      bool result = false;
      try
      {
        var gatewayid = GetCurrentGatewayId();
        var gateway = await gatewayRepository.FindAsync(gatewayid.Value);
        if (gateway == null)
        {
          throw new Exception("Current session does not belong to a gateway, or the gateway was removed.");
        }

        if (gateway.Status != GatewayStatus.Active)
        {
          if (gateway.Status != GatewayStatus.WaitingForConfirmation)
          {
            throw new Exception("The gateway is not waiting for confirmation.");
          }

          gateway.Status = GatewayStatus.Active;
          await gatewayRepository.UpdateAsync(gateway, true);
        }

        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task SetGatewayHealthStatus(ServiceHealthStatus healthStatus)
    {
      try
      {
        var gatewayid = GetCurrentGatewayId();
        var gateway = await gatewayRepository.FindAsync(gatewayid.Value);
        if (gateway == null)
        {
          throw new Exception("Current session does not belong to a gateway, or the gateway was removed.");
        }

        if (gateway.Status != GatewayStatus.Active)
        {
          throw new Exception("The gateway is not active.");
        }

        gateway.HealthStatus = healthStatus;
        gateway.LastHealthCheckTime = DateTime.UtcNow;
        await gatewayRepository.UpdateAsync(gateway);

        if (healthStatus == ServiceHealthStatus.Healthy && gateway.SelfHostEventBus)
        {
          await eventBusConnector.ConnectEventBus(gateway.EventBusId.Value);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task<GatewayEntity> GetCurrentGateway()
    {
      GatewayEntity result = null;
      try
      {
        var gatewayid = GetCurrentGatewayId();
        if (gatewayid != null)
        {
          var gateway = await gatewayRepository.FindAsync((Guid)gatewayid);
          if (gateway == null)
          {
            throw new Exception("The user is not a gateway impersonation, or the gateway was removed.");
          }

          if (gateway.Status != GatewayStatus.Active)
          {
            throw new Exception("The gateway is not active.");
          }

          result = gateway;
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    private async Task<string> SetGatewayMachineDetails(Guid gatewayId, string machineKey, byte[] certificatePem)
    {
      var gateway = await gatewayRepository.GetAsync(gatewayId);
      gateway.MachineHash = Convert.ToHexString(MD5.HashData(certificatePem));

      var gatewayDetails = await gatewayDetailsRepository.GetByGateway(gatewayId);
      gatewayDetails.MachineKey = machineKey;

      // Ensure that the certificate is valid and that the password is good.
      CertificateHelper.CreateCertificateFromPem(certificatePem, machineKey);
      gatewayDetails.CertificatePemBase64 = Convert.ToBase64String(certificatePem);

      await gatewayDetailsRepository.UpdateAsync(gatewayDetails, true);
      return gatewayDetails.ClientKey;
    }

    private async Task<GatewayRegistrationKeyEntity> ClaimGatewayRegistrationKey(string registrationKey)
    {
      var tenantId = await GetRegistrationKeyTenant(registrationKey);

      logger.Log.LogDebug($"Found registration key in tenant {tenantId}, validating.");

      using (CurrentTenant.Change(tenantId))
      {
        var keyEntity = await gatewayRegistrationKeysRepository.GetByKey(registrationKey);
        if (!keyEntity.IsValid())
        {
          throw new BusinessException(GatewayManagementErrorCodes.RegistrationKeyIsInvalid);
        }

        logger.Log.LogDebug($"Registration key for gateway {keyEntity.GatewayId} in tenant {tenantId} is valid.");

        keyEntity.Invalidated = true;
        await gatewayRegistrationKeysRepository.UpdateAsync(keyEntity, true);

        return keyEntity;
      }
    }

    private async Task<Guid?> GetRegistrationKeyTenant(string registrationKey)
    {
      var tenantKeys = await multiTenancyDomainService.ForEachTenant(async (tenantId) =>
      {
        var keyEntity = await gatewayRegistrationKeysRepository.GetByKey(registrationKey);
        if (keyEntity?.IsValid() == true)
        {
          return tenantId?.ToString() ?? "host";
        }
        else
        {
          logger.Log.LogDebug($"Key not found in {tenantId} tenant");
        }

        return null;
      });

      var matchedTenants = tenantKeys.Where(x => !x.IsNullOrWhiteSpace()).ToList();
      if (matchedTenants.Count != 1)
      {
        throw new BusinessException(GatewayManagementErrorCodes.RegistrationKeyIsInvalid);
      }

      var tenant = matchedTenants.First();
      return tenant == "host" ? null : Guid.Parse(tenant);
    }

    private Guid? GetCurrentGatewayId()
    {
      var apiSubjectName = currentPrincipalAccessor.Principal?.FindFirst("client_" + VPortalExtensionGrantsConsts.ApiKey.ApiKeyRefIdClaim)?.Value;
      var apiType = currentPrincipalAccessor.Principal?.FindFirst("client_" + VPortalExtensionGrantsConsts.ApiKey.ApiKeyTypeClaim)?.Value;
      if (apiSubjectName.IsNullOrEmpty() || apiType.IsNullOrEmpty())
      {
        return null;
      }

      if (Enum.Parse<ApiKeyType>(apiType) != ApiKeyType.Gateway)
      {
        return null;
      }

      if (Guid.TryParse(apiSubjectName, out var gatewayId))
      {
        return gatewayId;
      }

      return null;
    }
  }
}
