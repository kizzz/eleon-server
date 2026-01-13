using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Extensions;
using Common.Module.Helpers;
using Common.Module.Keys;
using Commons.Module.Messages.Identity;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Threading;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Identity.Module.IdentityServerExtensions.MachineKey
{
  public class MachineKeyGrantValidator : IExtensionGrantValidator
  {
    private readonly IDistributedEventBus apiKeyDomainService;
    private readonly ICurrentTenant currentTenant;
    private readonly IVportalLogger<MachineKeyGrantValidator> logger;

    public MachineKeyGrantValidator(
        IDistributedEventBus apiKeyDomainService,
        ICurrentTenant currentTenant,
        IVportalLogger<MachineKeyGrantValidator> logger)
    {
      this.apiKeyDomainService = apiKeyDomainService;
      this.currentTenant = currentTenant;
      this.logger = logger;
    }

    public string GrantType => VPortalExtensionGrantsConsts.Names.MachineKeyGrant;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
      try
      {
        string supposedEncryptedCompoundKey = context.Request.Raw.Get(VPortalExtensionGrantsConsts.MachineKey.MachineKeyParameter);
        if (supposedEncryptedCompoundKey is null)
        {
          throw new Exception("Machine key parameter is required for this grant.");
        }

        string supposedCompoundKey = EncryptionHelper.Decrypt(supposedEncryptedCompoundKey);
        ClientMachineCompoundKey compoundKey = ClientMachineCompoundKey.Parse(supposedCompoundKey);

        using (currentTenant.Change(compoundKey.TenantId))
        {
          var apiKeyEntity = (await apiKeyDomainService.RequestAsync<ValidApiKeyReponseMsg>(new GetValidApiKeyRequestMsg
          {
            ApiKey = compoundKey.Key,
          })).ApiKey;
          if (apiKeyEntity is null)
          {
            throw new Exception("Api key is not valid.");
          }

          string encryptedMachineKey = EncryptionHelper.Encrypt(compoundKey.MachineKey);
          var claims = new List<Claim>()
                    {
                        new Claim(VPortalExtensionGrantsConsts.MachineKey.MachineKeyClaim, encryptedMachineKey),
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyId, apiKeyEntity.Id.ToString()),
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyTypeClaim, apiKeyEntity.Type.ToString()),
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyRefIdClaim, apiKeyEntity.RefId.ToString()),
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyNameClaim, apiKeyEntity.Name),
                    };

          if (compoundKey.TenantId.HasValue)
          {
            claims.Add(new Claim(AbpClaimTypes.TenantId, compoundKey.TenantId.Value.ToString()));
          }

          foreach (var claim in claims)
          {
            context.Request.ClientClaims.Add(claim);
          }

          //context.Result = new GrantValidationResult(apiKeyEntity.Subject.ToString(), GrantType, claims);
          context.Result = new GrantValidationResult();
        }
      }
      catch (Exception ex)
      {
        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
        logger.CaptureAndSuppress(ex);
        return;
      }

    }
  }

}
