using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.EntityFrameworkCore;

namespace VPortal.SitesManagement.Module.Repositories
{
  public class ClientApplicationRepository : EfCoreRepository<SitesManagementDbContext, ApplicationEntity, Guid>, IClientApplicationRepository
  {
    private readonly IVportalLogger<ClientApplicationRepository> logger;

    public ClientApplicationRepository(
        IDbContextProvider<SitesManagementDbContext> dbContextProvider,
        IVportalLogger<ClientApplicationRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public override async Task<IQueryable<ApplicationEntity>> WithDetailsAsync()
    {
      var queryable = await GetQueryableAsync();
      return queryable
          .Include(t => t.Modules)
          .Include(t => t.Properties);
    }
  }
}


