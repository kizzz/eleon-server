using Common.Module.Constants;
using GatewayManagement.Module.Entities;
using GatewayManagement.Module.Repositories;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace VPortal.GatewayManagement.Module.DomainServices
{

  public class GatewayStaticKeyDomainService : DomainService
  {
    private readonly IGatewayRegistrationKeysRepository gatewayRegistrationKeysRepository;
    private readonly IVportalLogger<GatewayStaticKeyDomainService> logger;

    public GatewayStaticKeyDomainService(
        IGatewayRegistrationKeysRepository gatewayRegistrationKeysRepository,
        IVportalLogger<GatewayStaticKeyDomainService> logger)
    {
      this.gatewayRegistrationKeysRepository = gatewayRegistrationKeysRepository;
      this.logger = logger;
    }

    public async Task<bool> IsStaticKeyEnabled()
    {
      bool result = false;
      try
      {
        var key = await GetStaticKey();
        result = key != null;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<GatewayRegistrationKeyEntity> GetStaticKey()
    {
      GatewayRegistrationKeyEntity result = null;
      try
      {
        var keys = await gatewayRegistrationKeysRepository.GetMultiuseKeys();
        result = keys.FirstOrDefault(x => x.IsValid());
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetStaticKeyEnabled(bool shouldBeEnabled)
    {
      try
      {
        bool isEnabled = await IsStaticKeyEnabled();

        bool alreadyEnabled = isEnabled && shouldBeEnabled;
        if (alreadyEnabled)
        {
          throw new Exception("Static key is already enabled.");
        }

        bool alreadyDisabled = !isEnabled && !shouldBeEnabled;
        if (alreadyDisabled)
        {
          throw new Exception("Static key is already disabled.");
        }

        if (shouldBeEnabled)
        {
          var newKey = new GatewayRegistrationKeyEntity(GuidGenerator.Create())
          {
            ExpirationDate = DateTime.UtcNow.AddYears(100),
            GatewayId = null,
            Status = GatewayRegistrationKeyStatus.NotUsed,
            Multiuse = true,
            Key = Convert.ToHexString(SHA256.HashData(GuidGenerator.Create().ToString().GetBytes())),
          };

          await gatewayRegistrationKeysRepository.InsertAsync(newKey, true);
        }
        else
        {
          var key = await GetStaticKey();
          key.Invalidated = true;
          await gatewayRegistrationKeysRepository.UpdateAsync(key, true);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<bool> IsValidStaticKey(string key)
    {
      bool result = false;
      try
      {
        var keyEntity = await GetStaticKey();
        result =
            keyEntity != null
            && string.Compare(keyEntity.Key, key, StringComparison.InvariantCulture) == 0;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
