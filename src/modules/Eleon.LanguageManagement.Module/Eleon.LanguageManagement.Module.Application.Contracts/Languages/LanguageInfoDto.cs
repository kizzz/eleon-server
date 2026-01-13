namespace VPortal.LanguageManagement.Module.Languages
{
  public class LanguageInfoDto
  {
    public virtual string CultureName { get; set; }
    public virtual string UiCultureName { get; set; }
    public virtual string DisplayName { get; set; }
    public virtual string TwoLetterISOLanguageName { get; set; }
    public virtual string FlagIcon { get; set; }
  }
}
