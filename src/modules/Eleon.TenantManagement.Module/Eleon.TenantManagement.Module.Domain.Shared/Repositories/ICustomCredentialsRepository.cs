using Common.Module.Constants;
using Eleon.TenantManagement.Module.Eleon.TenantManagement.Module.Domain.Shared.Consts;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module.Repositories
{
  public interface ICustomCredentialsRepository : IBasicRepository<CustomCredentialsEntity, Guid>
  {
    Task<CustomCredentialsEntity> GetByValue(CustomCredentialsSet credentialsSet, string value);
  }
}
