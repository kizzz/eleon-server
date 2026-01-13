using System.Collections.Generic;
using Volo.Abp.Localization;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class LocalizationInformation
  {
    public List<string> LocalizationResources { get; set; }
    public List<LanguageInfo> Languages { get; set; }
    public string DefaultCulture { get; set; }
  }
}
