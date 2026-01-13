using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Helpers.Module;
public static class CollectionExtensions
{

  public static void AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue value)
  {
    if (!collection.ContainsKey(key))
    {
      collection.Add(key, value);
    }
  }

  public static string? Take(this IDictionary<string, string> dictionary, string key)
  {
    if (dictionary.TryGetValue(key, out var value))
    {
      dictionary.Remove(key);
      return value;
    }

    return null;
  }

  public static T? Take<T>(this IDictionary<string, string> dictionary, string key) where T : struct, IParsable<T>
  {
    if (dictionary.TryGetValue(key, out var value) && T.TryParse(value, null, out var result))
    {
      dictionary.Remove(key);
      return result;
    }

    return null;
  }
}
