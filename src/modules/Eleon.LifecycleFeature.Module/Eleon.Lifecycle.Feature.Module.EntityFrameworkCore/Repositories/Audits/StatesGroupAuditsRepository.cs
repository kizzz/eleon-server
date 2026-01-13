using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.EntityFrameworkCore;
using VPortal.Lifecycle.Feature.Module.Extensions;

namespace VPortal.Lifecycle.Feature.Module.Repositories.Audits
{
  public class StatesGroupAuditsRepository :
      EfCoreRepository<LifecycleFeatureDbContext, StatesGroupAuditEntity, Guid>, IStatesGroupAuditsRepository
  {
    private readonly IVportalLogger<StatesGroupAuditsRepository> logger;
    private readonly ICurrentUser currentUser;

    public StatesGroupAuditsRepository(
        IDbContextProvider<LifecycleFeatureDbContext> dbContextProvider,
        IVportalLogger<StatesGroupAuditsRepository> logger,
        ICurrentUser currentUser)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.currentUser = currentUser;
    }

    public override async Task<IQueryable<StatesGroupAuditEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).IncludeDetails();
    }

    public async Task<bool> Add(StatesGroupAuditEntity statesGroupAudit)
    {
      bool result = false;
      try
      {
        var dbSet = await GetDbSetAsync();
        await dbSet.AddAsync(statesGroupAudit);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<StatesGroupAuditEntity> GetByDocIdAsync(string documentObjectType, string documentId)
    {
      StatesGroupAuditEntity result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        var list = await dbSet
            .Where(
            (groupAudit) =>
                groupAudit.DocumentObjectType == documentObjectType
                && groupAudit.DocumentId == documentId)
            .IncludeDetails()
            .OrderByDescending(x => x.CreationTime)
            .ToListAsync();

        result = list.FirstOrDefault();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> Remove(Guid id)
    {
      bool result = false;
      try
      {
        await DeleteAsync(id);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<StateActorAuditEntity> GetStateActor(Guid id)
    {

      StateActorAuditEntity result = null;
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
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }

      return result;
    }

    public async Task<StateAuditEntity> GetState(Guid id)
    {

      StateAuditEntity result = null;
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
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<StatesGroupAuditEntity>>> GetReportListsAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null,
        DateTime? statusDateFilterStart = null,
        DateTime? statusDateFilterEnd = null,
        IList<string>? objectTypeFilter = null,
        Guid? userId = null,
        IList<string>? roles = null,
        Guid? statesTemplateGroupId = null)
    {

      try
      {
        var dbContext = await GetDbContextAsync();
        //string pattern = searchQuery == null ? null : $"%{searchQuery}%";

        var query = await dbContext.StatesGroupAudits
          .Where(x => x.Status != LifecycleStatus.Canceled)
            .IncludeDetails()
            .ToListAsync();

        var filtered = query
            .Where(x => x.CurrentState != null)
            //.Where(x => x.CurrentState.CurrentActor != null)
            .WhereIf(roles != null, x => x.CurrentState.CurrentActor != null && (x.CurrentState.CurrentActor.ActorType != LifecycleActorTypes.Role
                || roles.Contains(x.CurrentState.CurrentActor.RefId)))
            .WhereIf(userId.HasValue, x => x.CurrentState.CurrentActor != null && (x.CurrentState.CurrentActor.ActorType != LifecycleActorTypes.User
                || userId == Guid.Parse(x.CurrentState.CurrentActor.RefId)))
            .WhereIf(userId.HasValue, x => x.CurrentState.CurrentActor != null &&  (x.CurrentState.CurrentActor.ActorType != LifecycleActorTypes.Initiator
                || userId == x.CreatorId))
            .WhereIf(statesTemplateGroupId.HasValue, x => x.StatesGroupTemplateId == statesTemplateGroupId.Value)
            //.WhereIf(orgUnitFilter != null, x => orgUnitFilter.Contains((Guid)x.OrganizationUnitId))
            //.WhereIf(companyFilter != null, x => x.CompanyUid == companyFilter)
            //.WhereIf(dataSourceFilter != null, x => x.DataSourceUid == dataSourceFilter)
            .WhereIf(statusDateFilterStart != null,
                x => x.CurrentState.CurrentActor.StatusDate >= statusDateFilterStart)
            .WhereIf(statusDateFilterEnd != null,
                x => x.CurrentState.CurrentActor.StatusDate <= statusDateFilterEnd)
            .WhereIf(objectTypeFilter != null,
                x => objectTypeFilter.Contains(x.DocumentObjectType))
            //.WhereIf(searchQuery != null, x => searchQuery.Contains(x.DocEntry))
            //.WhereIf(searchQuery != null, x => EF.Functions.Like(x.DocEntry, pattern))
            ;

        var count = filtered.Count();

        var paginated = filtered
            .OrderByDescending(x => x.LastModificationTime)
            .Skip(skipCount)
            .Take(maxResultCount);

        return new KeyValuePair<long, List<StatesGroupAuditEntity>>(count, paginated.ToList());
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return new KeyValuePair<long, List<StatesGroupAuditEntity>>();
      //.Where(x => x.CurrentState.CurrentActor.ActorType == LifecycleActorTypes.User
      //    ? currentUser.Roles.Contains(x.CurrentState.CurrentActor.RefId)
      //    : true)
    }

    public async Task<List<string>> GetDocumentIdsByFilter(string documentObjectType, Guid? userId = null, List<string> roles = null, List<LifecycleStatus> lifecycleStatuses = null)
    {
      List<string> result = new List<string>();
      try
      {
        var dbContext = await GetDbContextAsync();
        var stateGroups = await dbContext.StatesGroupAudits
                    .Where(x => x.DocumentObjectType == documentObjectType)
                    .WhereIf(lifecycleStatuses != null && lifecycleStatuses.Count > 0, x => lifecycleStatuses.Contains(x.Status))
                    .IncludeDetails()
                    .ToListAsync();

        if (stateGroups != null && stateGroups.Count > 0)
        {
          foreach (var stateGroup in stateGroups)
          {
            if (stateGroup.States != null && stateGroup.States.Count > 0)
            {
              foreach (var state in stateGroup.States)
              {
                if (userId.HasValue && userId.Value != Guid.Empty)
                {
                  if (state.Actors.Any(report => report.IsActive &&
                  ((report.ActorType == LifecycleActorTypes.User && report.RefId == userId.Value.ToString())
                  || (report.ActorType == LifecycleActorTypes.Initiator && stateGroup.CreatorId == userId.Value)
                  || (report.ActorType == LifecycleActorTypes.Role && roles.Contains(report.RefId)))))
                  {
                    result.Add(stateGroup.DocumentId);
                  }
                }
                else
                {
                  if (state.Actors.Any(report => report.IsActive))
                  {
                    result.Add(stateGroup.DocumentId);
                  }
                }
              }
            }
            else
            {
              result.Add(stateGroup.DocumentId);
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }

      return result;
    }
  }
}
