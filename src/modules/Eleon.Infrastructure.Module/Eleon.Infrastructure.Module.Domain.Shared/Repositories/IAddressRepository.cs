using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Infrastructure.Module.Repositories
{
  public interface IAddressRepository : IBasicRepository<AddressEntity, Guid>
  {
    Task<AddressEntity> GetByHashCode(string hashCode);
  }
}
