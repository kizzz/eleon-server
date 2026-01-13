using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Razor;
using VPortal.Accounting.Module.Localization;

namespace VPortal.Accounting.Module
{
  public class ModuleTemplateDefinitionProvider : TemplateDefinitionProvider
  {
    public override void Define(ITemplateDefinitionContext context)
    {
      context.Add(
          new TemplateDefinition(AccountingTemplateConsts.ResendAccountInfoTemplate, typeof(AccountingResource))
              .WithRazorEngine()
              .WithVirtualFilePath(
                  "/Templates/ResendMessage.cshtml",
                  isInlineLocalized: true
              )
      );
    }
  }
}
