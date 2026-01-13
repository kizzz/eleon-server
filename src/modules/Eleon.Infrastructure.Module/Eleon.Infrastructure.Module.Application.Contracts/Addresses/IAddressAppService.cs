using System;
using System.Threading.Tasks;

namespace VPortal.Infrastructure.Module.Addresses
{
  public interface IAddressAppService
  {
    Task<AddressDto> GetAddressById(Guid addressId);
    Task<AddressDto> CreateAddress(AddressDto createdDto);
  }
}
