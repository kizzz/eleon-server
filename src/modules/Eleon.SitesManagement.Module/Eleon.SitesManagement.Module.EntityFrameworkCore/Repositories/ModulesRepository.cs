using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.EntityFrameworkCore;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Repositories
{
  public class ModulesRepository : EfCoreRepository<SitesManagementDbContext, ModuleEntity, Guid>, IModulesRepository
  {
    private readonly IVportalLogger<ModulesRepository> logger;

    public ModulesRepository(
        IDbContextProvider<SitesManagementDbContext> dbContextProvider,
        IVportalLogger<ModulesRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }
  }
}


