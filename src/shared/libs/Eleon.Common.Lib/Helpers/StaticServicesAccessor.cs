using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EleonsoftSdk.modules.Helpers.Module;
public static class StaticServicesAccessor
{
  private static IServiceProvider? _rootProvider = null;
  private static IServiceProvider? _rootProviderSource = null;
  private static IServiceScopeFactory? _scopeFactory = null;
  public static IServiceProvider RootProvider
  {
    get
    {
      if (_rootProvider == null)
      {
        throw new InvalidOperationException("ServiceProvider is not initialized. Please call InitializeFactory first.");
      }
      return _rootProvider;
    }
  }

  public static void Initialize(IServiceProvider rootProvider)
  {
    if (rootProvider == null)
    {
      return;
    }

    if (_rootProvider == null || !ReferenceEquals(_rootProviderSource, rootProvider))
    {
      _scopeFactory = rootProvider.GetRequiredService<IServiceScopeFactory>();
      _rootProvider = rootProvider;
      _rootProviderSource = rootProvider;
    }
  }

  public static T? GetService<T>()
  {
    if (_scopeFactory == null)
    {
      throw new InvalidOperationException("ServiceProvider is not initialized. Please call InitializeFactory first.");
    }

    using var scope = _scopeFactory.CreateScope();
    return scope.ServiceProvider.GetService<T>();
  }

  public static T GetRequiredService<T>()
  {
    if (_scopeFactory == null)
    {
      throw new InvalidOperationException("ServiceProvider is not initialized. Please call InitializeFactory first.");
    }

    using var scope = _scopeFactory.CreateScope();
    return scope.ServiceProvider.GetRequiredService<T>();
  }

  public static IConfiguration GetConfiguration()
  {
    return GetRequiredService<IConfiguration>();
  }

  public static IServiceScope CreateScope()
  {
    if (_scopeFactory == null)
    {
      throw new InvalidOperationException("ServiceProvider is not initialized. Please call InitializeFactory first.");
    }

    return _scopeFactory.CreateScope();
  }
}
