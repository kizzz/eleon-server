using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Helpers.Module;
public static class VersionHelper
{
  private const string VersionFileName = "version.txt";
  private const string DefaultVersion = "undefined";
  private const int MaxVersionLength = 32;


  public static string Version => GetVersion();


  private static string? _version = null;

  private static string GetVersion()
  {
    if (!string.IsNullOrEmpty(_version)) return _version;

    try
    {
      if (!System.IO.File.Exists(VersionFileName))
      {
        _version = DefaultVersion;
        return _version;
      }

      var fileInfo = new System.IO.FileInfo(VersionFileName);
      if (fileInfo.Length > MaxVersionLength)
      {
        _version = DefaultVersion;
        return _version;
      }

      _version = System.IO.File.ReadAllText(VersionFileName);
      if (string.IsNullOrWhiteSpace(_version))
      {
        _version = DefaultVersion;
      }
    }
    catch
    {
      _version = DefaultVersion;
    }

    return _version;
  }
}
