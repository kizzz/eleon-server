using Volo.Abp.Localization;

namespace Eleoncore.SDK.RequestLocalization
{
  public class LanguageDefaults
  {
    public static readonly List<LanguageInfo> DefaultLanguages = new()
        {
            new ("en", "en", "English"),
            new ("he", "he", "עִבְרִית"),
            new ("ar", "ar", "العربية"),
            new ("cs", "cs", "Čeština"),
            new ("fi", "fi", "Finnish"),
            new ("fr", "fr", "Français"),
            new ("hi", "hi", "Hindi"),
            new ("it", "it", "Italiano"),
            new ("pt-BR", "pt-BR", "Português"),
            new ("ru", "ru", "Русский"),
            new ("sl", "sl", "Slovenščina"),
            new ("sk", "sk", "Slovak"),
            new ("tr", "tr", "Türkçe"),
            new ("zh-Hans", "zh-Hans", "简体中文"),
            new ("zh-Hant", "zh-Hant", "繁體中文"),
        };

    public const string DefaultCulture = "en";
    public const string DevelopmentCulture = "dev";

    public static string[] GetDefaultSupportedCultures()
    {
      return DefaultLanguages.Select(x => x.CultureName).ToArray();//.Concat([DevelopmentCulture]).ToArray();
    }
  }
}
