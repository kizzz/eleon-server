using Common.Module.Constants;
using Common.Module.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using VPortal.LanguageManagement.Module.Entities;
using VPortal.LanguageManagement.Module.Repositories;

namespace VPortal.LanguageManagement.Module.Languages
{
  public class LanguageDataSeedContributor : IDataSeedContributor, ITransientDependency
  {
    private readonly ILanguageRepository languageRepository;
    private readonly IGuidGenerator guidGenerator;

    public LanguageDataSeedContributor(ILanguageRepository languageRepository, IGuidGenerator guidGenerator)
    {
      this.languageRepository = languageRepository;
      this.guidGenerator = guidGenerator;
    }


    public virtual async Task SeedAsync(DataSeedContext context)
    {
      await SeedDefaultLanguages();
    }

    private async Task SeedDefaultLanguages()
    {
      var existing = await languageRepository.GetListAsync();
      var defaults = LanguageDefaults.DefaultLanguages
          .Select(x => new LanguageEntity(guidGenerator.Create(), x)
          {
            IsDefault = x.CultureName == LanguageDefaults.DefaultCulture,
          })
          .ToList();

      var dif = existing.Difference(defaults, x => x.CultureName);

      await languageRepository.InsertManyAsync(dif.Added);
    }
  }
}
