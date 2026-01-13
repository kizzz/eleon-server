namespace VPortal.Core.Infrastructure.Module.FeatureSettings;

public class SetFeatureSettingDto
{
  public string Group { get; set; }
  public string Key { get; set; }
  public string Value { get; set; }
  public string Type { get; set; }
  public bool IsEncrypted { get; set; }
  public bool IsRequired { get; set; }
}
