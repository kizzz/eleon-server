using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Identity.Module.Sessions;
public interface ISessionAppService : IApplicationService
{
  Task<UserSessionDto> GetCurrentSessionAsync();
  Task<UserSessionDto> GetByIdAsync(string sessionId);
  Task<IReadOnlyList<UserSessionDto>> GetForCurrentUserAsync();
  Task<IReadOnlyList<UserSessionDto>> GetForUserAsync(string userId);

  Task RevokeCurrentAsync();
  Task RevokeAsync(string sessionId);
  Task RevokeAllAsync();
}
