using GatewayManagement.Module.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace GatewayManagement.Module.Repositories
{
  public interface IGatewayPrivateDetailsRepository : IBasicRepository<GatewayPrivateDetailsEntity, Guid>
  {
    Task<GatewayPrivateDetailsEntity> GetByGateway(Guid gatewayId);
  }
}
