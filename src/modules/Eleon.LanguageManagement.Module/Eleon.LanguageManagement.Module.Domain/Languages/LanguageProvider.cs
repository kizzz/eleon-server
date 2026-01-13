using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace VPortal.LanguageManagement.Module.Languages
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  [ExposeServices(typeof(ILanguageProvider))]
  internal class LanguageProvider : ILanguageProvider, ITransientDependency
  {
    private readonly LanguageDomainService languageDomainService;

    public LanguageProvider(LanguageDomainService languageDomainService)
    {
      this.languageDomainService = languageDomainService;
    }

    public async Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
    {
      return await languageDomainService.GetEnabledLanguages();
    }
  }
}
