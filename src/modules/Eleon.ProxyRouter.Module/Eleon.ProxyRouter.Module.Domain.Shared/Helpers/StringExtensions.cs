using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.ProxyRouter.Module.Eleon.ProxyRouter.Module.Domain.Shared.Helpers;
public static class StringExtensions
{
  public static string EnsureStartsWith(this string str, string prefix)
  {
    if (string.IsNullOrEmpty(str))
    {
      return prefix;
    }

    if (!str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
    {
      return prefix + str;
    }
    return str;
  }

  public static string EnsureEndsWith(this string str, string suffix)
  {
    if (string.IsNullOrEmpty(str))
    {
      return suffix;
    }
    if (!str.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
    {
      return str + suffix;
    }
    return str;
  }

  public static string ReplaceFirst(this string str, string oldValue, string newValue, StringComparison stringComparison = StringComparison.Ordinal)
  {
    int pos = str.IndexOf(oldValue, stringComparison);
    if (pos < 0)
    {
      return str;
    }
    return str.Substring(0, pos) + newValue + str.Substring(pos + oldValue.Length);
  }

  public static string SafeReplaceFirst(this string src, string from, string to, StringComparison stringComparison = StringComparison.Ordinal)
  {
    ArgumentNullException.ThrowIfNull(to);

    if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(from))
    {
      return src;
    }

    return src.ReplaceFirst(from, to, stringComparison);
  }

  public static bool IsEmpty(this string src)
  {
    return src == string.Empty;
  }

  public static bool IsAngularHostFile(this string str)
  {
    if (BlockListFiles.Contains(str))
    {
      return false;
    }

    if (str.StartsWith("/assets/"))
    {
      return true;
    }



    if (str.StartsWith('/'))
    {
      if (str.Substring(1).Contains('/'))
      {
        return false;
      }
    }
    else if (str.Contains('/'))
    {
      return false;
    }

    foreach (var ext in FileExtensions)
    {
      if (str.EndsWith(ext))
      {
        return true;
      }
    }
    return false;
  }

  private static string[] FileExtensions => [".html", ".json", ".js", ".mjs", ".js.map", ".css", ".ico"];
  private static string[] BlockListFiles => ["/ngsw.json", "/eleoncore-application-configuration.json", "/manifest.webmanifest"];
}
