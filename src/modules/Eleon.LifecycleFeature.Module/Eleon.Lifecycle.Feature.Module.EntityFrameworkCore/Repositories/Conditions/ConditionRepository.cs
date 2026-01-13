using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;
using VPortal.Lifecycle.Feature.Module.EntityFrameworkCore;

namespace VPortal.Lifecycle.Feature.Module.Repositories.Conditions
{
  public class ConditionRepository : EfCoreRepository<LifecycleFeatureDbContext, ConditionEntity, Guid>, IConditionRepository
  {
    private readonly IVportalLogger<ConditionRepository> logger;

    public ConditionRepository(IDbContextProvider<LifecycleFeatureDbContext> dbContextProvider,
        IVportalLogger<ConditionRepository> logger) : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public override async Task<IQueryable<ConditionEntity>> WithDetailsAsync()
    {
      IQueryable<ConditionEntity> queryable = await base.WithDetailsAsync();
      return queryable
          .Include(c => c.Rules);
      ;
    }
    public async Task<ConditionEntity> GetCondition(LifecycleConditionTargetType lifecycleConditionTargetType, Guid refId)
    {

      ConditionEntity result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet.FirstOrDefaultAsync(c => c.ConditionTargetType == lifecycleConditionTargetType && c.RefId == refId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }
  }
}
