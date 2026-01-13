using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.HealthCheckModule.Module.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.EntityFrameworkCore.Repositories;
public class HealthCheckReportRepository : EfCoreRepository<HealthCheckModuleDbContext, HealthCheckReport, Guid>, IHealthCheckReportRepository
{
  public HealthCheckReportRepository(IDbContextProvider<HealthCheckModuleDbContext> dbContextProvider) : base(dbContextProvider)
  {
  }


}
