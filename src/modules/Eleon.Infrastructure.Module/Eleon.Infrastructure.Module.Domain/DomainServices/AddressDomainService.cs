using Logging.Module;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.Repositories;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{

  public class AddressDomainService : DomainService, ISingletonDependency
  {

    private readonly IAddressRepository addressRepository;
    private readonly IVportalLogger<AddressDomainService> logger;

    public AddressDomainService(
        IAddressRepository addressRepository,
        IVportalLogger<AddressDomainService> logger)
    {
      this.addressRepository = addressRepository;
      this.logger = logger;
    }

    public async Task<AddressEntity> CreateAddress(AddressEntity createdEntity)
    {
      AddressEntity result = null;
      try
      {
        var hashCode = string.IsNullOrEmpty(createdEntity.AddressHashCode) ? GetHash(createdEntity) : createdEntity.AddressHashCode;
        var getWithSameHash = await addressRepository.GetByHashCode(hashCode);
        if (getWithSameHash == null)
        {
          Guid id = GuidGenerator.Create();
          AddressEntity newAddress = new AddressEntity(id);
          newAddress.Address2 = createdEntity.Address2;
          newAddress.EntityUid = createdEntity.EntityUid;
          newAddress.EntityName = createdEntity.EntityName;
          newAddress.AddressHashCode = hashCode;
          newAddress.StreetNo = createdEntity.StreetNo;
          newAddress.City = createdEntity.City;
          newAddress.Country = createdEntity.Country;
          newAddress.State = createdEntity.State;
          newAddress.ObjType = createdEntity.ObjType;
          newAddress.Street = createdEntity.Street;
          newAddress.AddrType = createdEntity.AddrType;
          newAddress.Address3 = createdEntity.Address3;
          newAddress.AddresType = createdEntity.AddresType;
          newAddress.ParentUid = createdEntity.ParentUid;
          newAddress.Building = createdEntity.Building;
          newAddress.ZipCode = createdEntity.ZipCode;
          newAddress.AddressName = createdEntity.AddressName;
          newAddress.CardCode = createdEntity.CardCode;

          result = await addressRepository.InsertAsync(newAddress, true);
          if (result == null)
          {
            throw new Exception("Unable to insert a new address.");
          }
        }
        else
        {
          result = getWithSameHash;
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<AddressEntity> GetAddressById(Guid addressId)
    {
      AddressEntity result = null;
      try
      {
        result = await addressRepository.FindAsync(addressId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    private string GetHash(AddressEntity address)
    {
      string inputString = address.CardCode.ToString() +
          address.Country +
          address.State +
          address.City +
          address.Street +
          address.StreetNo +
          address.Building +
          address.ZipCode +
          address.CardCode +
          address.Address2 +
          address.Address3;
      var sha = SHA256.Create();
      byte[] hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString));
      string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
      return hash;
    }
  }
}
