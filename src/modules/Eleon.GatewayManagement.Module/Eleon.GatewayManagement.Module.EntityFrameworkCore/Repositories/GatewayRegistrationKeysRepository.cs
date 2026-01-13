using Logging.Module;
using Microsoft.EntityFrameworkCore;
using GatewayManagement.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.GatewayManagement.Module.EntityFrameworkCore;

namespace GatewayManagement.Module.Repositories
{
  public class GatewayRegistrationKeysRepository : EfCoreRepository<GatewayManagementDbContext, GatewayRegistrationKeyEntity, Guid>, IGatewayRegistrationKeysRepository
  {
    private readonly IVportalLogger<GatewayRegistrationKeysRepository> _logger;

    public GatewayRegistrationKeysRepository(IDbContextProvider<GatewayManagementDbContext> dbContextProvider, IVportalLogger<GatewayRegistrationKeysRepository> logger)
        : base(dbContextProvider)
    {
      _logger = logger;
    }

    public async Task<List<GatewayRegistrationKeyEntity>> GetListByGateway(Guid gatewayId)
    {

      List<GatewayRegistrationKeyEntity> result = null;
      try
      {
        var db = await GetDbContextAsync();
        var filtered = db.GatewayRegistrationKeys.Where(x => x.GatewayId == gatewayId);
        result = await filtered.ToListAsync();
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<GatewayRegistrationKeyEntity> GetByKey(string key)
    {

      GatewayRegistrationKeyEntity result = null;
      try
      {
        var db = await GetDbContextAsync();
        var filtered = db.GatewayRegistrationKeys.Where(x => x.Key == key);
        result = await filtered.FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<GatewayRegistrationKeyEntity> GetLastAdded(Guid gatewayId)
    {

      GatewayRegistrationKeyEntity result = null;
      try
      {
        var db = await GetDbContextAsync();
        var filtered = db.GatewayRegistrationKeys
            .Where(x => x.GatewayId == gatewayId);
        var ordered = filtered
            .OrderByDescending(x => x.CreationTime);
        result = await ordered.FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<GatewayRegistrationKeyEntity>> GetMultiuseKeys()
    {

      List<GatewayRegistrationKeyEntity> result = null;
      try
      {
        var keys = await GetDbSetAsync();
        result = await keys.Where(x => x.Multiuse).ToListAsync();
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
