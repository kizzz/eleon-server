using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using GatewayManagement.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.GatewayManagement.Module.EntityFrameworkCore;

namespace GatewayManagement.Module.Repositories
{
  public class GatewayRepository : EfCoreRepository<GatewayManagementDbContext, GatewayEntity, Guid>, IGatewayRepository
  {
    private readonly IVportalLogger<GatewayRepository> logger;

    public GatewayRepository(
        IDbContextProvider<GatewayManagementDbContext> dbContextProvider,
        IVportalLogger<GatewayRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<GatewayEntity>> GetList(GatewayStatus? statusFilter = null)
    {

      List<GatewayEntity> result = null;
      try
      {
        var proxies = await GetDbSetAsync();
        var filtered = proxies.WhereIf(statusFilter != null, x => x.Status == statusFilter);
        var sorted = filtered.OrderByDescending(x => x.CreationTime);
        result = await sorted.ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public override async Task<IQueryable<GatewayEntity>> WithDetailsAsync()
    {
      var gateways = await GetDbSetAsync();
      return gateways;
    }
  }
}
