using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace VPortal.SitesManagement.Module.Helpers
{
  public static class UiModuleHelper
  {
    public static List<string> GetModules(string rootFolder)
    {
      List<string> modules = new List<string>();

      string[] manifestFiles = Directory.GetFiles(rootFolder, "module-configuration.manifest.json", SearchOption.AllDirectories);

      foreach (string file in manifestFiles)
      {
        try
        {
          string jsonContent = File.ReadAllText(file);
          JObject jsonObject = JObject.Parse(jsonContent);
          JObject modulesObject = (JObject)jsonObject["modules"];

          foreach (var module in modulesObject)
          {
            modules.Add(module.Key);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error reading or parsing {file}: {ex.Message}");
        }
      }

      return modules;
    }
  }
}


