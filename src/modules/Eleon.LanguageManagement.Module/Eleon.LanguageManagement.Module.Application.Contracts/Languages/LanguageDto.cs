using System;

namespace VPortal.LanguageManagement.Module.Languages
{
  public class LanguageDto
  {
    public Guid Id { get; set; }
    public string CultureName { get; set; }
    public string UiCultureName { get; set; }
    public string DisplayName { get; set; }
    public string TwoLetterISOLanguageName { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
  }
}
