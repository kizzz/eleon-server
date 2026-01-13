namespace VPortal.Infrastructure.Module.Domain.Localization
{
  /// <summary>
  /// A generalized interface used for reflection.
  /// </summary>
  public interface ICachedLocalizationResourceContributor
  {
    /// <summary>
    /// Invalidates the localization cache.
    /// That will resiult in a re-fetching of the dictionary on the next attempt to get a localized string.
    /// </summary>
    void InvalidateCache();
  }

  /// <summary>
  /// An cached localization resource that will resolve <typeparamref name="TKeysSource"/> to get keys.
  /// </summary>
  /// <typeparam name="TKeysSource">A type of the keys provider that will be resolved to get keys.</typeparam>
  public interface ICachedLocalizationResourceContributor<TKeysSource>
      : ICachedLocalizationResourceContributor
      where TKeysSource : ILocalizationKeysSource
  {
  }
}
