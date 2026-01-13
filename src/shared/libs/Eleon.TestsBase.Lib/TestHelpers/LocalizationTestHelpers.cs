using Microsoft.Extensions.Localization;
using NSubstitute;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for creating and configuring localizers for testing.
/// </summary>
public static class LocalizationTestHelpers
{
    /// <summary>
    /// Creates a mock IStringLocalizer with a single key-value pair.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="key">The localization key.</param>
    /// <param name="value">The localized value.</param>
    /// <returns>A mock IStringLocalizer configured with the specified key-value pair.</returns>
    public static IStringLocalizer<T> CreateLocalizer<T>(string key, string value)
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        localizer[key].Returns(new LocalizedString(key, value));
        return localizer;
    }

    /// <summary>
    /// Creates a mock IStringLocalizer with multiple key-value pairs.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="keyValuePairs">Dictionary of key-value pairs to configure.</param>
    /// <returns>A mock IStringLocalizer configured with the specified key-value pairs.</returns>
    public static IStringLocalizer<T> CreateLocalizer<T>(Dictionary<string, string> keyValuePairs)
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        
        foreach (var kvp in keyValuePairs)
        {
            localizer[kvp.Key].Returns(new LocalizedString(kvp.Key, kvp.Value));
        }
        
        return localizer;
    }
}

