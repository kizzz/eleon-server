using System;
using System.Linq;
using Volo.Abp.Localization;

namespace VPortal.Infrastructure.Module.Domain.Localization
{
  public static class LocalizationResourceExtensions
  {
    /// <summary>
    /// Instantiates a CachedLocalizationResourceContributor <typeparamref name="TKeysSource">
    /// and adds it to the list of resource's contributors.
    /// </summary>
    /// <typeparam name="TKeysSource">A type of keys source used to instantiate a cached contributor.</typeparam>
    /// <param name="resource">The resource to which the contributor will be added.</param>
    /// <exception cref="Exception">An exception will be thrown if a source of the same type is already registered as a contributor of the LocalizationResource.</exception>
    public static void AddCachedKeysSource<TKeysSource>(this LocalizationResource resource)
        where TKeysSource : ILocalizationKeysSource
    {
      var contributor = new CachedLocalizationResourceContributor<TKeysSource>();
      bool alreadyHasContributorOfThisType = resource.Contributors.Any(x => x.GetType() == contributor.GetType());
      if (alreadyHasContributorOfThisType)
      {
        throw new Exception($"The duplicate registration of resource provider {typeof(TKeysSource).FullName} for resource {resource.ResourceType.FullName}");
      }

      resource.Contributors.Add(contributor);
    }
  }
}
