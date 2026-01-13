using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.GatewayManagement.Module.StaticKeys
{
  public interface IGatewayStaticKeyAppService : IApplicationService
  {
    Task<string> GetStaticKey();
    Task SetStaticKeyEnabled(bool shouldBeEnabled);
  }
}
