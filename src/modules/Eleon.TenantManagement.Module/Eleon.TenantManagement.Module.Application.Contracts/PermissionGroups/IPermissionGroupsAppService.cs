using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.PermissionManagement;

namespace VPortal.TenantManagement.Module.PermissionGroups
{
  public interface IPermissionGroupsAppService : IApplicationService
  {
    Task<List<PermissionGroupCategory>> GetPermissionGroups();
    //Task<> CreateAsync(PermissionGroupDefinitionRecord createEntity)

  }
}
