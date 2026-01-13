using SharedModule.modules.Blob.Module.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharedModule.modules.Blob.Module;

/// <summary>
/// Service for caching VFS blob providers by key.
/// Thread-safe singleton service.
/// </summary>
public class VfsBlobProviderCacheService : ISingletonDependency, IDisposable
{
  private readonly ConcurrentDictionary<string, IVfsBlobProvider> _providers = new();

  /// <summary>
  /// Registers or replaces a blob provider by key.
  /// </summary>
  public void Register(string key, IVfsBlobProvider provider)
  {
    if (string.IsNullOrWhiteSpace(key))
      throw new ArgumentNullException(nameof(key));

    if (provider == null)
      throw new ArgumentNullException(nameof(provider));

    _providers[key] = provider;
  }

  /// <summary>
  /// Tries to get a blob provider by key.
  /// </summary>
  public bool TryGet(string key, [MaybeNullWhen(false)] out IVfsBlobProvider provider)
  {
    if (string.IsNullOrWhiteSpace(key))
    {
      provider = null;
      return false;
    }

    return _providers.TryGetValue(key, out provider);
  }

  /// <summary>
  /// Gets a blob provider or throws if not found.
  /// </summary>
  public IVfsBlobProvider Get(string key)
  {
    if (!_providers.TryGetValue(key, out var provider))
      throw new InvalidOperationException($"Blob provider '{key}' is not registered.");

    return provider;
  }

  /// <summary>
  /// Removes a blob provider by key.
  /// </summary>
  public bool Remove(string key)
  {
    return _providers.Remove(key, out _);
  }

  /// <summary>
  /// Clears all registered blob providers and disposes them.
  /// </summary>
  public void Dispose()
  {
    foreach (var provider in _providers.Values)
    {
      try
      {
        provider.Dispose();
      }
      catch
      {
        // Ignore disposal errors
      }
    }
    _providers.Clear();
  }
}

