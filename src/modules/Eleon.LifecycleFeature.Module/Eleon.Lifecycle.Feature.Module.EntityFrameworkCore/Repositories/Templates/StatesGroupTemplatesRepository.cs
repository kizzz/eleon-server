using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.EntityFrameworkCore;
using VPortal.Lifecycle.Feature.Module.Extensions;

namespace VPortal.Lifecycle.Feature.Module.Repositories.Templates
{
  public class StatesGroupTemplatesRepository : EfCoreRepository<LifecycleFeatureDbContext, StatesGroupTemplateEntity, Guid>, IStatesGroupTemplatesRepository
  {
    private readonly IDbContextProvider<LifecycleFeatureDbContext> dbContextProvider;
    private readonly IVportalLogger<StatesGroupTemplatesRepository> logger;

    public StatesGroupTemplatesRepository(IDbContextProvider<LifecycleFeatureDbContext> dbContextProvider, IVportalLogger<StatesGroupTemplatesRepository> logger) : base(dbContextProvider)
    {
      this.dbContextProvider = dbContextProvider;
      this.logger = logger;
    }

    public override async Task<IQueryable<StatesGroupTemplateEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).IncludeDetails();
    }

    public async Task<List<StatesGroupTemplateEntity>> GetListAsync(
        string documentObjectType,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0)
    {
      List<StatesGroupTemplateEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();

        result = await dbSet
           .Where(group => group.DocumentObjectType == documentObjectType)
           .OrderBy(sorting)
           .Skip(skipCount)
           .Take(maxResultCount)
           .ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }


      return result;
    }

    public async Task<KeyValuePair<long, List<StatesGroupTemplateEntity>>> GetPaginatedListAsync(
        string documentObjectType,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0)
    {
      KeyValuePair<long, List<StatesGroupTemplateEntity>> result = new();
      try
      {
        var dbSet = await GetDbSetAsync();

        var query = dbSet;//.Where(group => group.DocumentObjectType == documentObjectType)

        var count = await query.CountAsync();

        var paginated = await query

           .OrderBy(sorting)
           .Skip(skipCount)
           .Take(maxResultCount)
           .ToListAsync();
        result = new(count, paginated);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }


      return result;
    }

    public async Task<bool> Add(StatesGroupTemplateEntity statesGroupTemplate)
    {
      bool result = false;
      try
      {
        var dbSet = await GetDbSetAsync();
        await dbSet.AddAsync(statesGroupTemplate);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }


      return result;
    }

    public async Task<int> GetDocumentTypeGroupsCountAsync(string documentObjectType)
    {
      int result = 0;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where((group) => group.DocumentObjectType == documentObjectType)
            .CountAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }


      return result;
    }

    public async Task<StateTemplateEntity> GetState(Guid id)
    {
      StateTemplateEntity result = null;
      try
      {
        var query = await GetQueryableAsync();
        var group = await query
            .IncludeDetails()
            .FirstOrDefaultAsync(g => g.States.Any(state => state.Id == id));
        if (group != null)
        {
          result = group.States
              .Where(state => state.Id == id)
              .FirstOrDefault();
        }
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

    public async Task<StateActorTemplateEntity> GetStateActor(Guid id)
    {
      StateActorTemplateEntity result = null;
      try
      {
        var query = await GetQueryableAsync();
        var group = await query
            .IncludeDetails()
            .FirstOrDefaultAsync(g => g.States.Any(state => state.Actors.Any(actor => actor.Id == id)));
        if (group != null)
        {
          result = group.States
              .FirstOrDefault(state => state.Actors.Any(actor => actor.Id == id))
              ?.Actors
              ?.FirstOrDefault(actor => actor.Id == id);
        }
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
