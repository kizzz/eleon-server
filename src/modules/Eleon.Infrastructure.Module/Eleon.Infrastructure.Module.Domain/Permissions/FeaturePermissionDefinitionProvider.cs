using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace Common.Module.Permissions
{
  public class FeaturePermissionDefinitionProvider : ITransientDependency
  {
    private readonly IEnumerable<IFeaturePermissionDefinition> definitions;
    private readonly ILocalizableStringSerializer localizableStringSerializer;

    public FeaturePermissionDefinitionProvider(
        IEnumerable<IFeaturePermissionDefinition> definitions,
        ILocalizableStringSerializer localizableStringSerializer)
    {
      this.definitions = definitions;
      this.localizableStringSerializer = localizableStringSerializer;
    }

    public List<FeatureGroupDescription> GetDefinitions()
    {
      foreach (var d in definitions)
      {
        foreach (var g in d.Features)
        {
          g.DisplayName = Localize(d.LocalizationResource, g.LocalizationKey);
          foreach (var f in g.Children)
          {
            f.DisplayName = Localize(d.LocalizationResource, f.LocalizationKey);
            foreach (var pg in f.Permissions)
            {
              pg.DisplayName = Localize(d.LocalizationResource, pg.LocalizationKey);
              foreach (var p in pg.Children)
              {
                p.DisplayName = Localize(d.LocalizationResource, p.LocalizationKey);
              }
            }
          }
        }
      }

      return definitions.SelectMany(x => x.Features).ToList();
    }

    private string Localize(Type localizationResource, string localizationKey)
    {
      return localizableStringSerializer.Serialize(LocalizableString.Create(localizationResource, localizationKey));
    }
  }
}
