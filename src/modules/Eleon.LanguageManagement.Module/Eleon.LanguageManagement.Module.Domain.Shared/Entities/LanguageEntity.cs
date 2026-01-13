using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace VPortal.LanguageManagement.Module.Entities
{
  public class LanguageEntity : AggregateRoot<Guid>, IMultiTenant
  {
    protected LanguageEntity()
    {
    }

    public LanguageEntity(Guid id, string cultureName, string uiCultureName, string displayName, string twoLetterISOLanguageName)
    {
      Id = id;
      CultureName = cultureName;
      UiCultureName = uiCultureName;
      DisplayName = displayName;
      TwoLetterISOLanguageName = twoLetterISOLanguageName;
      IsEnabled = true;
    }

    public LanguageEntity(Guid id, LanguageInfo languageInfo)
    {
      Id = id;
      CultureName = languageInfo.CultureName;
      UiCultureName = languageInfo.UiCultureName;
      DisplayName = languageInfo.DisplayName;
      TwoLetterISOLanguageName = languageInfo.TwoLetterISOLanguageName;
      IsEnabled = true;
    }

    public Guid? TenantId { get; protected set; }
    public virtual string CultureName { get; protected set; }
    public virtual string UiCultureName { get; protected set; }
    public virtual string DisplayName { get; protected set; }
    public virtual string TwoLetterISOLanguageName { get; protected set; }
    public virtual bool IsEnabled { get; set; }
    public virtual bool IsDefault { get; set; }

    public LanguageInfo ToLanguageInfo()
        => new LanguageInfo(CultureName, UiCultureName, DisplayName);
  }
}
