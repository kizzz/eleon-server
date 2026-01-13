using Microsoft.Extensions.DependencyInjection;
using VPortal.Identity.Module.IdentityServerExtensions.ApiKey;
using VPortal.Identity.Module.IdentityServerExtensions.Impersonation;
using VPortal.Identity.Module.IdentityServerExtensions.MachineKey;
using VPortal.Identity.Module.IdentityServerExtensions.Token;

namespace VPortal.Identity.Module.IdentityServerExtensions
{
  public static class BuilderExtensions
  {
    public static IIdentityServerBuilder AddMachineKeyGrant(this IIdentityServerBuilder builder)
    {
      return builder.AddExtensionGrantValidator<MachineKeyGrantValidator>();
    }

    public static IIdentityServerBuilder AddApiKeyGrant(this IIdentityServerBuilder builder)
    {
      return builder.AddExtensionGrantValidator<ApiKeyGrantValidator>();
    }

    public static IIdentityServerBuilder AddCustomTokenRequestValidator(this IIdentityServerBuilder builder)
    {
      return builder.AddCustomTokenRequestValidator<VPortalTokenRequestValidator>();
    }

    public static IIdentityServerBuilder AddImpersonationGrant(this IIdentityServerBuilder builder)
    {
      return builder.AddExtensionGrantValidator<ImpersonationGrantValidator>();
    }
  }
}
