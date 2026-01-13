using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Razor;
using VPortal.Otp.Module.Localization;

namespace VPortal.Otp.Module
{
  public class OtpTemplateDefinitionProvider : TemplateDefinitionProvider
  {
    public override void Define(ITemplateDefinitionContext context)
    {
      context.Add(
          new TemplateDefinition(OtpTemplateConsts.OtpEmailTemplate, typeof(OtpResource))
              .WithRazorEngine()
              .WithVirtualFilePath(
                  "/Templates/OtpEmailTemplate.cshtml",
                  isInlineLocalized: true
              )
      );

      context.Add(
          new TemplateDefinition(OtpTemplateConsts.OtpEmailDebugTemplate, typeof(OtpResource))
              .WithRazorEngine()
              .WithVirtualFilePath(
                  "/Templates/OtpEmailDebugTemplate.cshtml",
                  isInlineLocalized: true
              )
      );
    }
  }
}
