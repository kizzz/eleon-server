using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Specifications;
using VPortal.Core.Infrastructure.Module.Entities;
using VPortal.Core.Infrastructure.Module.Specifications;
using VPortal.Infrastructure.Module.EntityFrameworkCore;

namespace VPortal.Core.Infrastructure.Module.Repositories
{
  public class FeatureSettingsRepository : EfCoreRepository<InfrastructureDbContext, FeatureSettingEntity, Guid>, IFeatureSettingsRepository
  {
    private readonly IVportalLogger<FeatureSettingsRepository> _logger;

    public FeatureSettingsRepository(IDbContextProvider<InfrastructureDbContext> dbContextProvider, IVportalLogger<FeatureSettingsRepository> logger) : base(dbContextProvider)
    {
      _logger = logger;
    }

    public async Task<List<FeatureSettingEntity>> GetListByGroupAsync(Guid? tenantId, string group, int skipCount, int maxResultCount, string sorting)
    {

      List<FeatureSettingEntity> result = new();
      try
      {
        DbSet<FeatureSettingEntity> dbSet = await GetDbSetAsync();

        result = await dbSet.Where(new FeatureSettingsByGroupSpecification(group)
            .And(new FeatureSettingsByTenantSpecification(tenantId)).ToExpression())
            .OrderBy(sorting).Skip(skipCount).Take(maxResultCount).ToListAsync();
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

    public async Task<long> GetCountListByGroupAsync(Guid? tenantId, string group)
    {
      long result = 0;
      try
      {
        DbSet<FeatureSettingEntity> dbSet = await GetDbSetAsync();

        result = await dbSet.Where(new FeatureSettingsByGroupSpecification(group)
            .And(new FeatureSettingsByTenantSpecification(tenantId)).ToExpression()).CountAsync();
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

    public async Task<FeatureSettingEntity> GetByKeyAsync(Guid? tenantId, string group, string key)
    {

      FeatureSettingEntity result = null;
      try
      {
        DbSet<FeatureSettingEntity> dbSet = await GetDbSetAsync();

        result = await dbSet.Where(new FeatureSettingsByGroupSpecification(group)
            .And(new FeatureSettingsByTenantSpecification(tenantId))
            .And(new FeatureSettingsByKeySpecification(key)).ToExpression())
        .SingleOrDefaultAsync();
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
