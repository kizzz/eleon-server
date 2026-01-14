using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Extensions;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.DocMessageLog.Module.Entities;
using VPortal.DocMessageLog.Module.EntityFrameworkCore;

namespace VPortal.DocMessageLog.Module.Repositories;

public class SystemLogRepository : EfCoreRepository<SystemLogDbContext, SystemLogEntity, Guid>, ISystemLogRepository
{
  private readonly IVportalLogger<SystemLogRepository> _logger;
  private readonly int _groupingTimeMinutes = 24 * 60;

  public SystemLogRepository(
      IDbContextProvider<SystemLogDbContext> dbContextProvider,
      IConfiguration configuration,
      IVportalLogger<SystemLogRepository> logger)
          : base(dbContextProvider)
  {
    _groupingTimeMinutes = configuration.GetValue("Logger:GroupingTimeMinutes", 24 * 60);
    _logger = logger;
  }

  public async Task<SystemLogEntity> WriteAsync(SystemLogEntity entity)
  {
    try
    {
      var hash = EleonsoftLogging.GenerateHash(entity.LogLevel, entity.Message ?? string.Empty, entity.TenantId, entity.ApplicationName ?? string.Empty);

      var since = DateTime.UtcNow.AddMinutes(-_groupingTimeMinutes);

      var existingEntity = await (await GetDbSetAsync())
          .Where(x => x.Hash == hash && x.CreationTime >= since)
          .OrderByDescending(x => x.CreationTime)
          .FirstOrDefaultAsync();

      if (existingEntity != null)
      {
        existingEntity.Count += 1;
        await UpdateAsync(existingEntity, true);
        return existingEntity;
      }

      entity.Hash = hash;
      await InsertAsync(entity, true);
      entity.Count = 1;
      return entity;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<KeyValuePair<int, List<SystemLogEntity>>> GetListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string searchQuery = null,
      SystemLogLevel? minLogLevelFilter = SystemLogLevel.Info,
      string initiatorFilter = null,
      string initiatorTypeFilter = null,
      string applicationNameFilter = null,
      DateTime? creationFromDateFilter = null,
      DateTime? creationToDateFilter = null,
      bool onlyUnread = false)
  {
    KeyValuePair<int, List<SystemLogEntity>> result = default;
    try
    {
      var dbSet = await GetDbSetAsync();

      string searchPattern = searchQuery == null ? null : $"%{searchQuery}%";
      var query = dbSet
          .WhereIf(minLogLevelFilter != null, x => x.LogLevel >= minLogLevelFilter)
          .WhereIf(!string.IsNullOrEmpty(initiatorFilter), x => x.InitiatorId == initiatorFilter)
          .WhereIf(!string.IsNullOrEmpty(initiatorTypeFilter), x => x.InitiatorType == initiatorTypeFilter)
          .WhereIf(!string.IsNullOrEmpty(applicationNameFilter), x => x.ApplicationName == applicationNameFilter)
          .WhereIf(creationFromDateFilter != null, x => x.LastModificationTime >= creationFromDateFilter.Value.Date)
          .WhereIf(creationToDateFilter != null, x => x.LastModificationTime <= creationToDateFilter.Value.Date)
          .WhereIf(searchQuery != null,
              x => EF.Functions.Like(x.Message, searchPattern)
              || EF.Functions.Like(x.Id.ToString(), searchPattern)
              || EF.Functions.Like(x.InitiatorId, searchPattern)
              || EF.Functions.Like(x.InitiatorType, searchPattern)
              || EF.Functions.Like(x.ApplicationName, searchPattern))
          .WhereIf(onlyUnread, x => !x.IsArchived);


      if (!string.IsNullOrEmpty(sorting))
      {
        query = query.OrderBy(sorting);
      }
      else
      {
        query = query.OrderByDescending(x => x.CreationTime);
      }

      var paged = query
          .Skip(skipCount)
          .Take(maxResultCount);

      var entities = await paged.ToListAsync();
      var count = await query.CountAsync();

      result = new(count, entities);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }

  public async Task<long> ShrinkAsync(DateTime olderThan)
  {

    try
    {

      var dbSet = await GetDbSetAsync();
      var count = await dbSet.CountAsync(x => x.CreationTime < olderThan);
      dbSet.RemoveRange(dbSet.Where(x => x.CreationTime < olderThan));
      await SaveChangesAsync(CancellationTokenProvider.Token);

      return count;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task MarkAllReadedAsync(SystemLogLevel? minLogLevelFilter = SystemLogLevel.Info)
  {
    try
    {

      var dbSet = await GetDbSetAsync();

      var q = dbSet
    .WhereIf(minLogLevelFilter != null, x => x.LogLevel >= minLogLevelFilter)
    .Where(x => !x.IsArchived);

      var affected = await q.ExecuteUpdateAsync(setters =>
          setters.SetProperty(x => x.IsArchived, true));
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task<UnresolvedSystemLogCount> GetTotalUnresolvedCountAsync(DateTime? creationFromDateFilter = null)
  {

    try
    {
      var dbSet = await GetDbSetAsync();

      var result = await dbSet
          .Where(x => !x.IsArchived &&
                     (x.LogLevel == SystemLogLevel.Critical ||
                      x.LogLevel == SystemLogLevel.Warning))
          .WhereIf(creationFromDateFilter != null, x => x.LastModificationTime.HasValue && x.LastModificationTime.Value > creationFromDateFilter.Value)
          .GroupBy(x => x.LogLevel)
          .Select(g => new
          {
            LogLevel = g.Key,
            Count = g.Count()
          })
          .ToListAsync();

      return new UnresolvedSystemLogCount
      {
        CriticalUnresolvedCount = result
              .FirstOrDefault(x => x.LogLevel == SystemLogLevel.Critical)?.Count ?? 0,

        WarningUnresolvedCount = result
              .FirstOrDefault(x => x.LogLevel == SystemLogLevel.Warning)?.Count ?? 0
      };
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

}
