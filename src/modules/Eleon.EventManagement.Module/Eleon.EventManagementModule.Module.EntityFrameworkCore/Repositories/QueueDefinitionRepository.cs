using System.Linq.Dynamic.Core;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.EventManagementModule.Module.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCor.Repositories;

public class QueueDefinitionRepository : EfCoreRepository<EventManagementModuleDbContext, QueueDefinitionEntity, Guid>, IQueueDefinitionRepository
{
  public QueueDefinitionRepository(IDbContextProvider<EventManagementModuleDbContext> dbContextProvider) : base(dbContextProvider)
  {
  }

  public async Task<List<QueueDefinitionEntity>> GetCustomListAsync(
      Guid? tenantId,
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      bool includeDetails = true,
      CancellationToken cancellationToken = default)
  {
    var query = includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();

    query = query
        .Where(f => f.TenantId == tenantId)
        .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(QueueDefinitionEntity.Name) + " desc" : sorting)
        .PageBy(skipCount, maxResultCount);

    return await query.ToListAsync(GetCancellationToken(cancellationToken));
  }
}
