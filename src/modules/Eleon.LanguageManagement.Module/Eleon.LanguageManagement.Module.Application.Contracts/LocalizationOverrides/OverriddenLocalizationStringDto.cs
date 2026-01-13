namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class OverriddenLocalizationStringDto
  {
    public string FullKey { get; set; }
    public string Key { get; set; }
    public string Base { get; init; }
    public string Target { get; init; }
    public bool IsOverride { get; init; }
    public string Resource { get; set; }
  }
}
