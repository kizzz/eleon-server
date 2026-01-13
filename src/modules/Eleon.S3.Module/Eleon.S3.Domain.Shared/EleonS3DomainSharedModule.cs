using EleonS3.Domain.Shared.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace EleonCore.Modules.S3;

[DependsOn(typeof(AbpValidationModule))]
public class EleonS3DomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(o =>
        {
            o.Resources.Add<S3Resource>("en");
        });
    }
}
