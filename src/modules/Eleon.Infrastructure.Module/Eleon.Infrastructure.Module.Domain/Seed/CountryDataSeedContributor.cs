using Nager.Country;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.Repositories;

namespace VPortal.Infrastructure.Module.Domain.Seed
{
  public class CountryDataSeedContributor : IDataSeedContributor, ITransientDependency
  {
    private readonly ICountryRepository countryRepository;
    private readonly IGuidGenerator guidGenerator;
    private readonly ICurrentTenant currentTenant;

    public CountryDataSeedContributor(
        ICountryRepository countryRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
      this.countryRepository = countryRepository;
      this.guidGenerator = guidGenerator;
      this.currentTenant = currentTenant;
    }


    public async Task SeedAsync(DataSeedContext context)
    {
      using (currentTenant.Change(context?.TenantId))
      {
        if (await countryRepository.GetCountAsync() > 0)
        {
          return;
        }

        List<CountryEntity> countryEntities = new List<CountryEntity>();
        var countryProvider = new CountryProvider();
        var countries = countryProvider.GetCountries();
        foreach (var countryInfo in countries)
        {
          CountryEntity countryEntity = new CountryEntity(guidGenerator.Create());
          countryEntity.Name = countryInfo.CommonName;
          countryEntity.Code = countryInfo.Alpha2Code.ToString();
          countryEntities.Add(countryEntity);
        }

        await countryRepository.InsertManyAsync(countryEntities, true);
      }
    }
  }
}
