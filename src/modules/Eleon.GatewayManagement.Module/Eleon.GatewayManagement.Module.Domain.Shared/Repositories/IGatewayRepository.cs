using Common.Module.Constants;
using GatewayManagement.Module.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace GatewayManagement.Module.Repositories
{
  public interface IGatewayRepository : IBasicRepository<GatewayEntity, Guid>
  {
    Task<List<GatewayEntity>> GetList(GatewayStatus? statusFilter = null);
  }
}
