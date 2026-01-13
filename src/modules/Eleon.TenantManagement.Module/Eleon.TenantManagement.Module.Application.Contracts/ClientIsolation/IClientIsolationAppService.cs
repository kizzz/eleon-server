using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.TenantManagement.Module.ClientIsolation;

namespace VPortal.TenantManagement.Module.TenantIsolation
{
  public interface IClientIsolationAppService : IApplicationService
  {
    Task<bool> SetTenantIsolation(SetTenantIsolationRequestDto request);
    Task<bool> SetUserIsolation(SetUserIsolationRequestDto request);
    Task<bool> SetTenantIpIsolationSettings(SetIpIsolationRequestDto request);
    Task<UserIsolationSettingsDto> GetUserIsolationSettings(Guid userId);
    Task<bool> ValidateClientIsolation(ValidateClientIsolationDto validateClientIsolationDto);
  }
}
