using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.EntityFrameworkCore;
using VPortal.Vpn;

namespace VPortal.Repositories
{
  public class VpnSettingsRepository : EfCoreRepository<VPortalDbContext, VpnServerSettingsEntity, Guid>, IVpnSettingsRepository
  {
    private readonly IVportalLogger<VpnSettingsRepository> logger;

    public VpnSettingsRepository(
        IDbContextProvider<VPortalDbContext> dbContextProvider,
        IVportalLogger<VpnSettingsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<VpnServerSettingsEntity> GetByNetworkName(string networkName)
    {
      VpnServerSettingsEntity result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet.FirstOrDefaultAsync(x => x.NetworkName == networkName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
