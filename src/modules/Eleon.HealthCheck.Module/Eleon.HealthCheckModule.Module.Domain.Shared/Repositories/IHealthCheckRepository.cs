using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Repositories;
public interface IHealthCheckRepository : IBasicRepository<HealthCheck, Guid>
{
  Task<HealthCheck> GetFullByIdAsync(Guid id);
  Task<KeyValuePair<long, List<HealthCheckReport>>> GetReportsAsync(
      Guid healthCheckId,
      string filter = null,
      string sorting = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      CancellationToken cancellationToken = default);

  Task<KeyValuePair<long, List<HealthCheck>>> GetListAsync(
      string sorting = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      string type = null,
      string initator = null,
      DateTime? minTime = null,
      DateTime? maxTime = null,
      bool includeDetails = true);
}
