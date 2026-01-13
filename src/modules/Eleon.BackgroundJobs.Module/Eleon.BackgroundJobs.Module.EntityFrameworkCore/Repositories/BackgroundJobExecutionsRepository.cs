using BackgroundJobs.Module.Extensions;
using Logging.Module;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;

namespace VPortal.BackgroundJobs.Module.Repositories
{
  public class BackgroundJobExecutionsRepository :
      EfCoreRepository<BackgroundJobsDbContext, BackgroundJobExecutionEntity, Guid>,
      IBackgroundJobExecutionsRepository
  {
    private readonly IDbContextProvider<BackgroundJobsDbContext> dbContextProvider;
    private readonly IVportalLogger<BackgroundJobExecutionsRepository> logger;
    public BackgroundJobExecutionsRepository(IDbContextProvider<BackgroundJobsDbContext> dbContextProvider,
        IVportalLogger<BackgroundJobExecutionsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public override async Task<IQueryable<BackgroundJobExecutionEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).IncludeMessages();
    }
  }
}
