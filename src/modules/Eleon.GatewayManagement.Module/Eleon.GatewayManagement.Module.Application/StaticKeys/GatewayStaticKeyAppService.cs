using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.GatewayManagement.Module.DomainServices;

namespace VPortal.GatewayManagement.Module.StaticKeys
{
  [Authorize]
  public class GatewayStaticKeyAppService : GatewayManagementBaseAppService, IGatewayStaticKeyAppService
  {
    private readonly IVportalLogger<GatewayStaticKeyAppService> logger;
    private readonly GatewayStaticKeyDomainService gatewayStaticKeyDomain;

    public GatewayStaticKeyAppService(
        GatewayStaticKeyDomainService gatewayStaticKeyDomain,
        IVportalLogger<GatewayStaticKeyAppService> logger)
    {
      this.gatewayStaticKeyDomain = gatewayStaticKeyDomain;
      this.logger = logger;
    }

    public async Task<string> GetStaticKey()
    {
      string result = null;
      try
      {
        bool isEnabled = await gatewayStaticKeyDomain.IsStaticKeyEnabled();
        if (isEnabled)
        {
          var key = await gatewayStaticKeyDomain.GetStaticKey();
          result = key.Key;
        }
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
        await gatewayStaticKeyDomain.SetStaticKeyEnabled(shouldBeEnabled);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
