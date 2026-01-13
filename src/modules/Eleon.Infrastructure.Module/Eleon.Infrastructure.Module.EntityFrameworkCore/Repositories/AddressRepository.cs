using Logging.Module;
using Sentry;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.EntityFrameworkCore;

namespace VPortal.Infrastructure.Module.Repositories
{
  public class AddressRepository : EfCoreRepository<InfrastructureDbContext, AddressEntity, Guid>, IAddressRepository
  {
    private readonly IVportalLogger<AddressRepository> logger;

    public AddressRepository(
        IDbContextProvider<InfrastructureDbContext> dbContextProvider,
        IVportalLogger<AddressRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<AddressEntity> GetByHashCode(string hashCode)
    {
      AddressEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = dbContext.Addresses.Where(x => x.AddressHashCode == hashCode).FirstOrDefault();
      }
      catch (Exception e)
      {
        logger.Capture(e);
        SentrySdk.CaptureException(e);
      }

      return result;
    }
  }
}
