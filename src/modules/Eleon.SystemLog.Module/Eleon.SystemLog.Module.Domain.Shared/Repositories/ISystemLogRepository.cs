using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.DocMessageLog.Module.Entities;

namespace VPortal.DocMessageLog.Module.Repositories;

public interface ISystemLogRepository : IBasicRepository<SystemLogEntity, Guid>
{
  Task<SystemLogEntity> WriteAsync(SystemLogEntity entity);
  Task<KeyValuePair<int, List<SystemLogEntity>>> GetListAsync(
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
      bool onlyUnread = false);

  Task<long> ShrinkAsync(DateTime olderThan);
  Task MarkAllReadedAsync(SystemLogLevel? minLogLevelFilter = SystemLogLevel.Info);
  Task<UnresolvedSystemLogCount> GetTotalUnresolvedCountAsync(DateTime? creationFromDateFilter = null);
}
