using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for creating and working with memory cache in tests.
/// </summary>
public static class MemoryCacheTestHelpers
{
    /// <summary>
    /// Creates a real MemoryCache instance for testing.
    /// </summary>
    /// <returns>A MemoryCache instance.</returns>
    public static IMemoryCache CreateMemoryCache()
    {
        var options = Options.Create(new MemoryCacheOptions());
        return new MemoryCache(options);
    }

    /// <summary>
    /// Creates a MemoryCache instance with custom options.
    /// </summary>
    /// <param name="options">Memory cache options.</param>
    /// <returns>A MemoryCache instance with the specified options.</returns>
    public static IMemoryCache CreateMemoryCache(MemoryCacheOptions options)
    {
        return new MemoryCache(Options.Create(options));
    }
}


