using Eleoncore.SDK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using VPortal.SdkAuthorization;

namespace EleonsoftAbp.EleonsoftPermissions;
public static class EleonsoftPermissionsExtension
{
  public static IServiceCollection AddEleonsoftPermissions(this IServiceCollection services)
  {
    services.Replace(ServiceDescriptor.Transient<IAuthorizationPolicyProvider, EleonsoftSdkPolicyProvider>());
    services.AddTransient<IAuthorizationPolicyProvider, EleonsoftSdkPolicyProvider>();
    services.AddTransient<IAbpAuthorizationPolicyProvider, CustomAbpAuthorizationPolicyProvider>();
    services.AddTransient<IAuthorizationHandler, EleonsoftSdkPermissionRequirementHandler>();
    services.AddTransient<EleonsoftSdkPermissionCacheCleaner>();

    return services;
  }

  public static IServiceCollection ForbidApiKeysWithoutAuthorize(this IServiceCollection services)
  {
    services.AddTransient<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
    return services;
  }
}
