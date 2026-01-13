using Eleon.Auth.App;
using Volo.Abp.Modularity;
using VPortal.ApplicationConfiguration.Module;
using VPortal.ExternalLinkModule;
using VPortal.HealthCheckModule;
using VPortal.Identity.Module;
using VPortal.Otp.Module;

namespace VPortal;

[DependsOn(
    typeof(EleonHostModule),
    typeof(AuthMvcModule),
    typeof(ApplicationConfigurationModuleCollector),
    typeof(OtpModuleCollector),
    typeof(ExternalLinkModuleCollector),
    typeof(IdentityQueryingModuleCollector)
    )]
public class AuthHostModule : AbpModule
{
}
