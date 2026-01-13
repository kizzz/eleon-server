namespace Common.Module.Helpers
{
  public class ManifestHelper
  {
    public static string ConfigurationPath = "module.manifest.json";

    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public static async Task<T> GetManifestSetting<T>(string sectionPathKey)
    {
      await semaphore.WaitAsync();
      try
      {
        await EnsureManifestExists();

        string json = await File.ReadAllTextAsync(ConfigurationPath);
        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        return jsonObj.SelectToken(sectionPathKey.Replace(':', '.'))?.ToObject<T>();
      }
      finally
      {
        semaphore.Release();
      }
    }

    public static async Task AddOrUpdateManifestSetting<T>(string sectionPathKey, T value)
    {
      await semaphore.WaitAsync();
      try
      {
        await EnsureManifestExists();

        string json = await File.ReadAllTextAsync(ConfigurationPath);
        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        SetValueRecursively(sectionPathKey, jsonObj, value);

        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
        await File.WriteAllTextAsync(ConfigurationPath, output);
      }
      finally
      {
        semaphore.Release();
      }
    }

    private static async Task EnsureManifestExists()
    {
      if (!File.Exists(ConfigurationPath))
      {
        await File.WriteAllTextAsync(ConfigurationPath, "{}");
      }
    }

    private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
    {
      // split the string at the first ':' character
      var remainingSections = sectionPathKey.Split(":", 2);

      var currentSection = remainingSections[0];
      if (remainingSections.Length > 1)
      {
        // continue with the procress, moving down the tree
        var nextSection = remainingSections[1];
        SetValueRecursively(nextSection, jsonObj[currentSection], value);
      }
      else
      {
        // we've got to the end of the tree, set the value
        jsonObj[currentSection] = value;
      }
    }
  }
}
