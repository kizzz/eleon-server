using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.Repositories
{
  public interface IControlDelegationRepository : IBasicRepository<ControlDelegationEntity, Guid>
  {
    Task<List<ControlDelegationEntity>> GetActiveControlDelegationsByUser(Guid userId, DateTime asForDate);
    Task<KeyValuePair<int, List<ControlDelegationEntity>>> GetControlDelegationsByUser(Guid userId, int skip, int take);
    Task<List<ControlDelegationEntity>> GetActiveControlDelegationsToUser(Guid userId, DateTime asForDate);
    Task<List<ControlDelegationEntity>> GetControlDelegationsRelatedToUser(Guid userId);
  }
}
