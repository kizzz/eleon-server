using Volo.Abp.DependencyInjection;

namespace Common.Module.Permissions
{
  public interface IFeaturePermissionDefinition : ITransientDependency
  {
    public Type LocalizationResource { get; }
    public List<FeatureGroupDescription> Features { get; }
  }
}
