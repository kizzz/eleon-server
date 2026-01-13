using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace VPortal;

[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
public class VPortalBrandingProvider : DefaultBrandingProvider
{
  public override string AppName => "EleonSphere";
}
