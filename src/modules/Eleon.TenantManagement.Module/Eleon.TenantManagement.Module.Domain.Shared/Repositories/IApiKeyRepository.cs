using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module.Repositories
{
  public interface IApiKeyRepository : IBasicRepository<ApiKeyEntity, Guid>
  {
    Task<ApiKeyEntity> GetByKey(string key);
    Task<ApiKeyEntity> GetBySubject(ApiKeyType keyType, string subject);
  }
}
