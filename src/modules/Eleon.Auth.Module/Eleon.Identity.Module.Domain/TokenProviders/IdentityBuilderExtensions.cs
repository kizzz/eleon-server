using Microsoft.AspNetCore.Identity;

namespace VPortal.Identity.Module.TokenProviders
{
  public static class IdentityBuilderExtensions
  {
    public static IdentityBuilder AddApiKeyTokenProvider(this IdentityBuilder builder)
    {
      var userType = builder.UserType;
      var provider = typeof(ApiKeyTokenProvider<>).MakeGenericType(userType);
      return builder.AddTokenProvider(ApiKeyTokenProviderOptions.DefaultName, provider);
    }
  }
}
