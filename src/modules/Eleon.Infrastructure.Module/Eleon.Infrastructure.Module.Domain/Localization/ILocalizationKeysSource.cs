using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.Infrastructure.Module.Domain.Localization
{
  /// <summary>
  /// A provider of the localization keys and values for the default culture.
  /// </summary>
  public interface ILocalizationKeysSource : ITransientDependency
  {
    Task<IReadOnlyDictionary<string, string>> GetLocalizationKeysWithDefaultsAsync();
  }
}
