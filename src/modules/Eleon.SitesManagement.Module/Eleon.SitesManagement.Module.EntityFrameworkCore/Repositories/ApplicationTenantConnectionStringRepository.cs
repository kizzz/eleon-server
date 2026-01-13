using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.EntityFrameworkCore;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.EntityFrameworkCore.Repositories;
public class ApplicationTenantConnectionStringRepository : EfCoreRepository<SitesManagementDbContext, ApplicationTenantConnectionStringEntity, Guid>, IApplicationTenantConnectionStringRepository
{
  public ApplicationTenantConnectionStringRepository(IDbContextProvider<SitesManagementDbContext> dbContextProvider) : base(dbContextProvider)
  {
  }
}


