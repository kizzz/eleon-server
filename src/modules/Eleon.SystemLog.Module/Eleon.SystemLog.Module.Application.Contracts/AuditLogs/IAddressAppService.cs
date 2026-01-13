using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace VPortal.Infrastructure.Module.AuditLogs
{
  public interface IAuditLogAppService
  {
    Task<PagedResultDto<AuditLogHeaderDto>> GetAuditLogList(AuditLogListRequestDto input);
    Task<AuditLogDto> GetAuditLogById(Guid id);
    Task<PagedResultDto<EntityChangeDto>> GetEntityChangeList(EntityChangeListRequestDto input);
    Task<EntityChangeDto> GetEntityChangeById(Guid id);
    Task<bool> AddAuditAsync(AuditLogDto input);
  }
}
