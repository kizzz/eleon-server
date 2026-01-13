using GatewayManagement.Module.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace GatewayManagement.Module.Repositories
{
  public interface IGatewayRegistrationKeysRepository : IBasicRepository<GatewayRegistrationKeyEntity, Guid>
  {
    Task<List<GatewayRegistrationKeyEntity>> GetListByGateway(Guid gatewayId);
    Task<GatewayRegistrationKeyEntity> GetLastAdded(Guid gatewayId);
    Task<GatewayRegistrationKeyEntity> GetByKey(string key);
    Task<List<GatewayRegistrationKeyEntity>> GetMultiuseKeys();
  }
}
