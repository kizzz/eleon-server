using System.Collections.Generic;
using VPortal.LanguageManagement.Module.Languages;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class LocalizationInformationDto
  {
    public List<string> LocalizationResources { get; set; }
    public List<LanguageInfoDto> Languages { get; set; }
    public string DefaultCulture { get; set; }
  }
}
