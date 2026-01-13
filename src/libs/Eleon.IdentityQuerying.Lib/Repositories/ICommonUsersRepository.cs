using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace VPortal.TenantManagement.Module.Repositories
{
  public interface ICommonUsersRepository
  {
    public Task<List<IdentityUser>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string filter = null,
        string permissions = null,
        List<Guid> ignoredUsers = null);
    public Task<int> GetCountAsync(
        string filter = null,
        string permissions = null,
        List<Guid> ignoredUsers = null);
    public Task<List<IdentityUser>> GetByIds(List<Guid> ids);
  }
}
