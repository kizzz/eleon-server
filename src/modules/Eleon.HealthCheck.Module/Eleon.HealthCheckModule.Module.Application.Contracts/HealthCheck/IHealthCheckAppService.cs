using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public interface IHealthCheckAppService : IApplicationService
{
  Task<HealthCheckDto> CreateAsync(CreateHealthCheckDto request);
  Task<HealthCheckDto> SendAsync(SendHealthCheckDto request);
  Task<FullHealthCheckDto> GetByIdAsync(Guid id);
  Task<bool> AddReportBulkAsync(AddHealthCheckReportBulkDto request);

  Task<PagedResultDto<HealthCheckDto>> GetListAsync(HealthCheckRequestDto request);

  Task<HealthCheckReportDto> AddReportAsync(AddHealthCheckReportDto request);
}
