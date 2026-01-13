using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.HealthCheckModule.Module.EntityFrameworkCore;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.EntityFrameworkCore.Repositories;
public class HealthCheckRepository : EfCoreRepository<HealthCheckModuleDbContext, HealthCheck, Guid>, IHealthCheckRepository
{
  public HealthCheckRepository(IDbContextProvider<HealthCheckModuleDbContext> dbContextProvider) : base(dbContextProvider)
  {
  }

  public async Task<KeyValuePair<long, List<HealthCheck>>> GetListAsync(
      string sorting = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      string type = null,
      string initator = null,
      DateTime? minTime = null,
      DateTime? maxTime = null,
      bool includeDetails = true)
  {
    var dbSet = await GetDbSetAsync();

    var query = dbSet
        .AsNoTracking()
        .WhereIf(!string.IsNullOrWhiteSpace(type), x => x.Type == type)
        .WhereIf(!string.IsNullOrWhiteSpace(initator), x => x.InitiatorName == initator)
        .WhereIf(minTime.HasValue, x => x.CreationTime >= minTime.Value)
        .WhereIf(maxTime.HasValue, x => x.CreationTime <= maxTime.Value)
        ;


    var totalCount = await query.CountAsync();

    if (string.IsNullOrWhiteSpace(sorting))
    {
      sorting = nameof(HealthCheck.CreationTime) + " DESC";
    }

    var result = await query
        .OrderBy(sorting)
        .Skip(skipCount)
        .Take(maxResultCount)
        .ToListAsync();

    if (includeDetails)
    {
      var checksIds = result.Select(x => x.Id).ToList();
      var currentTenantReports = await (await GetDbContextAsync())
          .HealthCheckReports
          .AsNoTracking()
          .Include(x => x.ExtraInformation)
          .Where(x => checksIds.Contains(x.HealthCheckId))
          .ToListAsync();

      if (CurrentTenant.Id != null)
      {
        using (CurrentTenant.Change(null))
        {
          var hostTenantReports = await (await GetDbContextAsync())
          .HealthCheckReports
          .AsNoTracking()
          .Include(x => x.ExtraInformation)
          .Where(x => x.IsPublic)
          .Where(x => checksIds.Contains(x.HealthCheckId))
          .ToListAsync();

          foreach (var check in result)
          {
            check.Reports.AddRange(hostTenantReports.Where(x => x.HealthCheckId == check.Id));
          }
        }
      }

      foreach (var check in result)
      {
        check.Reports.AddRange(currentTenantReports.Where(x => x.HealthCheckId == check.Id));
      }
    }

    return KeyValuePair.Create((long)totalCount, result);
  }

  public async Task<KeyValuePair<long, List<HealthCheckReport>>> GetReportsAsync(
      Guid healthCheckId,
      string filter = null,
      string sorting = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      CancellationToken cancellationToken = default)
  {
    var dbSet = (await GetDbContextAsync()).HealthCheckReports.Include(x => x.ExtraInformation);
    var query = dbSet.Where(r => r.HealthCheckId == healthCheckId);
    var totalCount = await query.CountAsync(cancellationToken);

    if (!string.IsNullOrWhiteSpace(filter))
    {
      query = query.Where(r => r.CheckName.Contains(filter) || r.ServiceName.Contains(filter) || r.Message.Contains(filter));
    }

    if (string.IsNullOrWhiteSpace(sorting))
    {
      sorting = nameof(HealthCheckReport.CreationTime) + " DESC";
    }

    var items = await query
        .OrderBy(sorting)
        .Skip(skipCount)
        .Take(maxResultCount)
        .ToListAsync();

    return new KeyValuePair<long, List<HealthCheckReport>>(totalCount, items);
  }

  public async Task<HealthCheck> GetFullByIdAsync(Guid id)
  {
    var entity = await (await WithDetailsAsync())
        .AsNoTracking()
        .FirstOrDefaultAsync(h => h.Id == id);

    var currentTenantReports = await (await GetDbContextAsync())
            .HealthCheckReports
            .AsNoTracking()
            .Include(x => x.ExtraInformation)
            .Where(x => x.HealthCheckId == id)
            .ToListAsync();

    entity.Reports.AddRange(currentTenantReports);

    if (CurrentTenant.Id != null)
    {
      using (CurrentTenant.Change(null))
      {
        var hostReports = await (await GetDbContextAsync()).HealthCheckReports
            .AsNoTracking()
            .Include(x => x.ExtraInformation)
            .Where(x => x.HealthCheckId == id && x.IsPublic)
            .ToListAsync();

        entity.Reports.AddRange(hostReports);
      }
    }

    return entity;
  }


  public override async Task<IQueryable<HealthCheck>> WithDetailsAsync()
  {
    var query = (await base.WithDetailsAsync());
    return query;
  }
}
