using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Razor;
using VPortal.Lifecycle.Feature.Module.Localization;

namespace VPortal.Lifecycle.Feature.Module
{
  public class ModuleTemplateDefinitionProvider : TemplateDefinitionProvider
  {
    public override void Define(ITemplateDefinitionContext context)
    {
      context.Add(
          new TemplateDefinition(ModuleTemplateConsts.LifecycleWaitingForApproval, typeof(LifecycleFeatureModuleResource))
              .WithRazorEngine()
              .WithVirtualFilePath(
                  "/Templates/LifecycleWaitingForApproval.cshtml",
                  isInlineLocalized: true
              )
      );
      context.Add(
          new TemplateDefinition(ModuleTemplateConsts.LifecycleComplete, typeof(LifecycleFeatureModuleResource))
              .WithRazorEngine()
              .WithVirtualFilePath(
                  "/Templates/LifecycleComplete.cshtml",
                  isInlineLocalized: true
              )
      );
    }
  }
}
