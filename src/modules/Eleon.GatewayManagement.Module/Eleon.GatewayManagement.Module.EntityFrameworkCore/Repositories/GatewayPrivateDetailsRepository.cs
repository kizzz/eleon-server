using Logging.Module;
using Microsoft.EntityFrameworkCore;
using GatewayManagement.Module.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.GatewayManagement.Module.EntityFrameworkCore;

namespace GatewayManagement.Module.Repositories
{
  public class GatewayPrivateDetailsRepository : EfCoreRepository<GatewayManagementDbContext, GatewayPrivateDetailsEntity, Guid>, IGatewayPrivateDetailsRepository
  {
    private readonly IVportalLogger<GatewayPrivateDetailsRepository> _logger;

    public GatewayPrivateDetailsRepository(IDbContextProvider<GatewayManagementDbContext> dbContextProvider, IVportalLogger<GatewayPrivateDetailsRepository> logger)
        : base(dbContextProvider)
    {
      _logger = logger;
    }

    public async Task<GatewayPrivateDetailsEntity> GetByGateway(Guid gatewayId)
    {

      GatewayPrivateDetailsEntity result = null;
      try
      {
        var db = await GetDbContextAsync();
        result = await db.GatewayPrivateDetails.SingleAsync(x => x.GatewayId == gatewayId);
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
