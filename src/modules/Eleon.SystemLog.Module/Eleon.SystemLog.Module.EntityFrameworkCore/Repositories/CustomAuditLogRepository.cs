using System.Net;
using System.Linq.Dynamic.Core;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Auditing;

namespace ModuleCollector.Infrastructure.Module.Infrastructure.Module.EntityFrameworkCore.Repositories;

[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IAuditLogRepository), typeof(CustomAuditLogRepository))]
public class CustomAuditLogRepository : EfCoreAuditLogRepository, IAuditLogRepository, IScopedDependency
{
  public CustomAuditLogRepository(IDbContextProvider<IAuditLoggingDbContext> dbContextProvider) : base(dbContextProvider)
  {
  }

  public override async Task<long> GetCountAsync(DateTime? startTime = null, DateTime? endTime = null, string httpMethod = null, string url = null, string clientId = null, Guid? userId = null, string userName = null, string applicationName = null, string clientIpAddress = null, string correlationId = null, int? maxExecutionDuration = null, int? minExecutionDuration = null, bool? hasException = null, HttpStatusCode? httpStatusCode = null, CancellationToken cancellationToken = default(CancellationToken))
  {
    var query = await GetDbSetAsync();

    return await query
        .WhereIf(startTime.HasValue, x => x.ExecutionTime >= startTime.Value)
        .WhereIf(endTime.HasValue, x => x.ExecutionTime <= endTime.Value)
        .WhereIf(!string.IsNullOrEmpty(httpMethod), x => x.HttpMethod == httpMethod)
        .WhereIf(!string.IsNullOrEmpty(url), x => x.Url.Contains(url))
        .WhereIf(userId.HasValue, x => x.UserId == userId.Value)
        .WhereIf(!string.IsNullOrEmpty(userName), x => x.UserName.Contains(userName))
        .WhereIf(!string.IsNullOrEmpty(applicationName), x => x.ApplicationName.Contains(applicationName))
        .WhereIf(!string.IsNullOrEmpty(clientIpAddress), x => x.ClientIpAddress == clientIpAddress)
        .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
        .WhereIf(maxExecutionDuration.HasValue, x => x.ExecutionDuration <= maxExecutionDuration.Value)
        .WhereIf(minExecutionDuration.HasValue, x => x.ExecutionDuration >= minExecutionDuration.Value)
        .WhereIf(hasException.HasValue, x => (x.Exceptions != null && x.Exceptions != "") == hasException.Value)
        .WhereIf(httpStatusCode.HasValue, x => x.HttpStatusCode == (int)httpStatusCode.Value)
        .CountAsync();
  }

  public override async Task<List<AuditLog>> GetListAsync(
      string sorting = null,
      int maxResultCount = 50,
      int skipCount = 0,
      DateTime? startTime = null,
      DateTime? endTime = null,
      string httpMethod = null,
      string url = null,
      string clientId = null,
      Guid? userId = null,
      string userName = null,
      string applicationName = null,
      string clientIpAddress = null,
      string correlationId = null,
      int? maxExecutionDuration = null,
      int? minExecutionDuration = null,
      bool? hasException = null,
      HttpStatusCode? httpStatusCode = null,
      bool includeDetails = false,
      CancellationToken cancellationToken = default)
  {
    var query = includeDetails ? await WithDetailsAsync() : await GetDbSetAsync();

    query = query
        .WhereIf(startTime.HasValue, x => x.ExecutionTime >= startTime.Value)
        .WhereIf(endTime.HasValue, x => x.ExecutionTime <= endTime.Value)
        .WhereIf(!string.IsNullOrEmpty(httpMethod), x => x.HttpMethod == httpMethod)
        .WhereIf(!string.IsNullOrEmpty(url), x => x.Url.Contains(url))
        .WhereIf(userId.HasValue, x => x.UserId == userId.Value)
        .WhereIf(!string.IsNullOrEmpty(userName), x => x.UserName.Contains(userName))
        .WhereIf(!string.IsNullOrEmpty(applicationName), x => x.ApplicationName.Contains(applicationName))
        .WhereIf(!string.IsNullOrEmpty(clientIpAddress), x => x.ClientIpAddress == clientIpAddress)
        .WhereIf(!string.IsNullOrEmpty(correlationId), x => x.CorrelationId == correlationId)
        .WhereIf(maxExecutionDuration.HasValue, x => x.ExecutionDuration <= maxExecutionDuration.Value)
        .WhereIf(minExecutionDuration.HasValue, x => x.ExecutionDuration >= minExecutionDuration.Value)
        .WhereIf(hasException.HasValue, x => (x.Exceptions != null && x.Exceptions != "") == hasException.Value)
        .WhereIf(httpStatusCode.HasValue, x => x.HttpStatusCode == (int)httpStatusCode.Value);

    if (string.IsNullOrWhiteSpace(sorting))
      sorting = nameof(AuditLog.ExecutionTime) + " desc";

    query = query.OrderBy(sorting);
    query = query.Skip(skipCount).Take(maxResultCount);

    return await query.ToListAsync(cancellationToken);
  }
}
