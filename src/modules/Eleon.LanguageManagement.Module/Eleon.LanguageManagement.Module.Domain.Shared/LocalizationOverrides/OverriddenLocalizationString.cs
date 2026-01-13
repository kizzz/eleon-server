using Microsoft.Extensions.Localization;
using System;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class OverriddenLocalizationString
  {
    public string FullKey => $"{Resource}::{Key}";
    public string Key { get; set; }
    public string Base { get; init; }
    public string Target { get; init; }
    public bool IsOverride { get; init; }
    public string Resource { get; set; }

    public OverriddenLocalizationString(LocalizedString baseString, LocalizedString targetString, string resource, bool isOverride)
    {
      if (baseString.Name != targetString.Name)
      {
        throw new Exception("Base and target strings should correspond to the same localization key.");
      }

      Key = baseString.Name;
      Base = baseString.Value;
      Target = targetString.Value;
      Resource = resource;
      IsOverride = isOverride;
    }
  }
}
