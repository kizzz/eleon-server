using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Otp.Module.Entities;
using VPortal.Otp.Module.EntityFrameworkCore;

namespace VPortal.Otp.Module.Repositories
{
  public class OtpRepository :
      EfCoreRepository<OtpDbContext, OtpEntity, Guid>,
      IOtpRepository
  {
    private readonly IVportalLogger<OtpDbContext> logger;

    public OtpRepository(
        IVportalLogger<OtpDbContext> logger,
        IDbContextProvider<OtpDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<OtpEntity> FindByKey(string key)
    {
      OtpEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.Otps
            .Where(x => x.Key == key)
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<OtpEntity> GetOtpByRecipient(string recipient)
    {
      OtpEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.Otps
            .Where(x => x.Recipient == recipient)
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
