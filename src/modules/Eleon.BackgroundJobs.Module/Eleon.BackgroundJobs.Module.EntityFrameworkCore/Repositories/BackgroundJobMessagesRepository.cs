using Logging.Module;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;

namespace VPortal.BackgroundJobs.Module.Repositories
{
  public class BackgroundJobMessagesRepository : EfCoreRepository<BackgroundJobsDbContext, BackgroundJobMessageEntity, Guid>, IBackgroundJobMessagesRepository
  {
    private readonly IDbContextProvider<BackgroundJobsDbContext> dbContextProvider;
    private readonly IVportalLogger<BackgroundJobMessagesRepository> logger;
    public BackgroundJobMessagesRepository(IDbContextProvider<BackgroundJobsDbContext> dbContextProvider, IVportalLogger<BackgroundJobMessagesRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }
  }

}
