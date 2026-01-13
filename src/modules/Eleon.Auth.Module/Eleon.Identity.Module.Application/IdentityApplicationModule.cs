using EleonsoftAbp.EleonsoftPermissions;
using Identity.Module.Application.IdentityServerServices;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Account;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace VPortal.Identity.Module;

[DependsOn(
    typeof(IdentityDomainModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpAccountApplicationModule)
    )]
public class IdentityApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<IdentityApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<IdentityApplicationModule>(validate: true);
    });

    context.Services.Replace(ServiceDescriptor.Scoped<IRedirectUriValidator, CustomRedirectUriValidator>());
    context.Services.AddTransient<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
  }
}
