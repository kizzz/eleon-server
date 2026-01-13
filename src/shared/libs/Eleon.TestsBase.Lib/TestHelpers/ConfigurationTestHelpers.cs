using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for creating IConfiguration instances for testing.
/// </summary>
public static class ConfigurationTestHelpers
{
    /// <summary>
    /// Creates an IConfiguration from a dictionary of key-value pairs.
    /// </summary>
    /// <param name="settings">Dictionary of configuration key-value pairs.</param>
    /// <returns>An IConfiguration instance.</returns>
    public static IConfiguration CreateConfiguration(Dictionary<string, string> settings)
    {
        var builder = new ConfigurationBuilder();
        builder.AddInMemoryCollection(settings);
        return builder.Build();
    }

    /// <summary>
    /// Creates an IConfiguration from a JSON string.
    /// </summary>
    /// <param name="json">JSON configuration string.</param>
    /// <returns>An IConfiguration instance.</returns>
    public static IConfiguration CreateConfigurationFromJson(string json)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)));
        return builder.Build();
    }

    /// <summary>
    /// Creates an IConfiguration with appsettings-style nested structure.
    /// </summary>
    /// <param name="sectionName">The root section name (e.g., "ApplicationConfiguration").</param>
    /// <param name="settings">Dictionary of nested key-value pairs.</param>
    /// <returns>An IConfiguration instance.</returns>
    public static IConfiguration CreateNestedConfiguration(string sectionName, Dictionary<string, object> settings)
    {
        var flatSettings = new Dictionary<string, string>();
        FlattenDictionary(settings, sectionName, flatSettings);
        return CreateConfiguration(flatSettings);
    }

    private static void FlattenDictionary(Dictionary<string, object> source, string prefix, Dictionary<string, string> target)
    {
        foreach (var kvp in source)
        {
            var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}:{kvp.Key}";
            
            if (kvp.Value is Dictionary<string, object> nested)
            {
                FlattenDictionary(nested, key, target);
            }
            else if (kvp.Value is System.Collections.IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (item is Dictionary<string, object> itemDict)
                    {
                        FlattenDictionary(itemDict, $"{key}:{i}", target);
                    }
                    else
                    {
                        target[$"{key}:{i}"] = item?.ToString() ?? string.Empty;
                    }
                }
            }
            else
            {
                target[key] = kvp.Value?.ToString() ?? string.Empty;
            }
        }
    }
}


