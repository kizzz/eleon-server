using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace VPortal.TenantManagement.Module.ControlDelegations
{
  public interface IControlDelegationAppService
  {
    Task<bool> AddControlDelegation(CreateControlDelegationRequestDto request);
    Task<List<ControlDelegationDto>> GetActiveControlDelegationsByUser();
    Task<List<ControlDelegationDto>> GetActiveControlDelegationsToUser();
    Task<PagedResultDto<ControlDelegationDto>> GetControlDelegationsByUser(int skip, int take);
    Task<ControlDelegationDto> GetControlDelegation(Guid delegationId);
    Task<bool> SetControlDelegationActiveState(Guid delegationId, bool isActive);
    Task<bool> UpdateControlDelegation(UpdateControlDelegationRequestDto request);
  }
}
