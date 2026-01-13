using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module.Repositories
{
  public class ControlDelegationRepository : EfCoreRepository<TenantManagementDbContext, ControlDelegationEntity, Guid>, IControlDelegationRepository
  {
    private readonly IVportalLogger<ControlDelegationRepository> logger;

    public ControlDelegationRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<ControlDelegationRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<ControlDelegationEntity>> GetActiveControlDelegationsByUser(Guid userId, DateTime asForDate)
    {
      List<ControlDelegationEntity> result = null;
      try
      {
        var controlDelegations = await GetDbSetAsync();
        result = await controlDelegations
            .Where(x => x.UserId == userId && x.Active)
            .OrderByDescending(x => x.DelegationStartDate)
            .ThenByDescending(x => x.CreationTime)
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<KeyValuePair<int, List<ControlDelegationEntity>>> GetControlDelegationsByUser(Guid userId, int skip, int take)
    {
      KeyValuePair<int, List<ControlDelegationEntity>> result = default;
      try
      {
        var controlDelegations = await GetDbSetAsync();
        var filtered = controlDelegations
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.DelegationStartDate)
            .ThenByDescending(x => x.CreationTime);

        var paged = await filtered
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var count = await filtered.CountAsync();

        result = new(count, paged);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ControlDelegationEntity>> GetActiveControlDelegationsToUser(Guid userId, DateTime asForDate)
    {
      List<ControlDelegationEntity> result = null;
      try
      {
        var controlDelegations = await GetDbSetAsync();
        result = await controlDelegations
            .Where(x => x.DelegatedToUserId == userId && x.Active && asForDate >= x.DelegationStartDate && (x.DelegationEndDate == null || asForDate <= x.DelegationEndDate))
            .OrderByDescending(x => x.DelegationStartDate)
            .ThenByDescending(x => x.CreationTime)
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ControlDelegationEntity>> GetControlDelegationsRelatedToUser(Guid userId)
    {
      List<ControlDelegationEntity> result = null;
      try
      {
        var controlDelegations = await GetDbSetAsync();
        result = await controlDelegations
            .Where(x => x.DelegatedToUserId == userId || x.UserId == userId)
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public override async Task<IQueryable<ControlDelegationEntity>> WithDetailsAsync()
    {
      return (await GetDbSetAsync()).Include(x => x.DelegationHistory);
    }
  }
}
