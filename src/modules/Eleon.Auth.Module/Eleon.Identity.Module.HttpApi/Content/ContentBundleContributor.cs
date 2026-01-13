
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace VPortal.Content
{
  public class ContentBundleContributor : BundleContributor
  {
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
      context.Files.Add("/styles/site.css");
    }
  }
}
