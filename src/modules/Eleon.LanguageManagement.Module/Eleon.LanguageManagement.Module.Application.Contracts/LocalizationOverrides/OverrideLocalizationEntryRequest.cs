namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class OverrideLocalizationEntryRequest
  {
    public string ResourceName { get; set; }
    public string Key { get; set; }
    public string CultureName { get; set; }
    public string NewValue { get; set; }
  }
}
