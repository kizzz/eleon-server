using Logging.Module;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Localization;
using Volo.Abp.Uow;
using VPortal.Infrastructure.Module.Domain.Localization;

namespace VPortal.Infrastructure.Module.Domain.DomainServices;


public class LocalizationDomainService : DomainService
{
  private readonly IVportalLogger<LocalizationDomainService> logger;
  private readonly IOptions<AbpLocalizationOptions> abpLocalizationOptions;

  public LocalizationDomainService(
      IVportalLogger<LocalizationDomainService> logger,
      IOptions<AbpLocalizationOptions> abpLocalizationOptions)
  {
    this.logger = logger;
    this.abpLocalizationOptions = abpLocalizationOptions;
  }

  /// <summary>
  /// Invalidates the cache for a given keys source in a given localization resource.
  /// </summary>
  /// <typeparam name="TLocalizationResource">The type of the localization resource where the keys source should be found.</typeparam>
  /// <typeparam name="TKeysSource">The typoe of the keys source to invalidate cache in.</typeparam>
  /// <returns>The boolean determining whether the cache was successfully invalidated.</returns>
  public async Task<bool> InvalidateCachedLocalizationResourceCache<TLocalizationResource, TKeysSource>()
  {

    bool result = false;
    try
    {
      var resource = abpLocalizationOptions.Value.Resources.Get<TLocalizationResource>();
      var resourceContributorType = typeof(ICachedLocalizationResourceContributor<>).MakeGenericType(typeof(TKeysSource));
      var contributor = resource.Contributors
          .FirstOrDefault(x => resourceContributorType.IsInstanceOfType(x));
      if (contributor == null)
      {
        throw new Exception($"No contributors that work with {typeof(TKeysSource)} were found in the {typeof(TLocalizationResource)}");
      }

      contributor.As<ICachedLocalizationResourceContributor>().InvalidateCache();

      result = true;
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }

    return result;
  }
}
