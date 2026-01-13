namespace Common.Module.Keys
{
  public class LightweightStorageKey : CompoundKey
  {
    public string SettingsGroup { get; set; }
    public string BlobName { get; set; }

    public override string Key => CombineKeyParts(SettingsGroup, BlobName);

    public LightweightStorageKey(string settingsGroup, string blobName)
    {
      SettingsGroup = settingsGroup;
      BlobName = blobName;
    }

    public static LightweightStorageKey Parse(string key)
    {
      var parts = key.Split(KeyDelimeter);
      if (parts.Length != 2)
      {
        throw new ArgumentException("Invalid key format");
      }

      return new LightweightStorageKey(parts[0], parts[1]);
    }
  }
}
