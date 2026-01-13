using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Localization;

namespace VPortal.Infrastructure.Module.Domain.Localization
{
  /// <summary>
  /// A default implementation of the <see cref="ILocalizationResourceContributor{TKeysSource}"/>.
  /// </summary>
  /// <typeparam name="TKeysSource">A type of the keys provider that will be resolved to get keys.</typeparam>
  public class CachedLocalizationResourceContributor<TKeysSource>
      : ILocalizationResourceContributor, ICachedLocalizationResourceContributor<TKeysSource>
      where TKeysSource : ILocalizationKeysSource
  {
    protected IServiceProvider serviceProvider;
    protected ILogger<CachedLocalizationResourceContributor<TKeysSource>> logger;
    private string defaultCulture;
    protected Dictionary<string, LocalizedString> cachedDictionary;
    protected readonly object locker = new();

    public bool IsDynamic => true;

    public virtual void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
      if (cultureName != defaultCulture)
      {
        return;
      }

      try
      {
        GetLocalizationDictionary(cultureName).Fill(dictionary);
      }
      catch (Exception)
      {
        logger.LogError($"Could not get the localization dictionary for culture {cultureName} in contributor {GetType()}");
      }
    }

    public virtual LocalizedString GetOrNull(string cultureName, string name)
    {
      return GetLocalizationDictionary(cultureName)?.GetOrNull(name);
    }

    public virtual void Initialize(LocalizationResourceInitializationContext context)
    {
      serviceProvider = context.ServiceProvider;
      logger = serviceProvider.GetRequiredService<ILogger<CachedLocalizationResourceContributor<TKeysSource>>>();
      defaultCulture = context.Resource.DefaultCultureName;
    }

    public void InvalidateCache()
    {
      lock (locker)
      {
        cachedDictionary = null;
      }
    }

    private ILocalizationDictionary GetLocalizationDictionary(string cultureName)
        => new StaticLocalizationDictionary(cultureName, GetDictionary());

    private Dictionary<string, LocalizedString> GetDictionary()
    {
      var dictionary = cachedDictionary;
      if (dictionary != null)
      {
        return dictionary;
      }

      lock (locker)
      {
        dictionary = cachedDictionary;
        if (dictionary != null)
        {
          return dictionary;
        }

        dictionary = cachedDictionary = CreateDictionary();
      }

      return dictionary;
    }

    private Dictionary<string, LocalizedString> CreateDictionary()
    {
      var sourceDictionary = ResolveSourceDictionary();
      var localizaitonDictionary = sourceDictionary
          .ToDictionary(
          x => x.Key,
          x => new LocalizedString(x.Key, x.Value.NormalizeLineEndings()));

      return localizaitonDictionary;
    }

    private IReadOnlyDictionary<string, string> ResolveSourceDictionary()
    {
      var source = serviceProvider.GetRequiredService<TKeysSource>();
      var sourceDictionary = source.GetLocalizationKeysWithDefaultsAsync().GetAwaiter().GetResult();
      return sourceDictionary;
    }

    public Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
      Fill(cultureName, dictionary);
      return Task.CompletedTask;
    }

    public async Task<IEnumerable<string>> GetSupportedCulturesAsync()
    {
      return Array.Empty<string>().ToList();
    }
  }
}
