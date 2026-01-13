using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace VPortal.Infrastructure.Module.SecurityLogs
{
  public interface ISecurityLogAppService
  {
    Task<PagedResultDto<SecurityLogDto>> GetSecurityLogList(SecurityLogListRequestDto input);
    Task<FullSecurityLogDto> GetSecurityLogByIdAsync(Guid id);
  }
}
