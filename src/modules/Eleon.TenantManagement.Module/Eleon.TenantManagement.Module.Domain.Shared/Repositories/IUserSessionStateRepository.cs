using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module.Repositories
{
  public interface IUserSessionStateRepository : IBasicRepository<UserSessionStateEntity, Guid>
  {
    Task<UserSessionStateEntity> GetByUser(Guid userId);
  }
}
