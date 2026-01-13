using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace VPortal.Vpn
{
  public interface IVpnSettingsRepository : IBasicRepository<VpnServerSettingsEntity, Guid>
  {
    Task<VpnServerSettingsEntity> GetByNetworkName(string networkName);
  }
}
