namespace VPortal.Core.Infrastructure.Module.IdentityServer
{
  public class VportalIdentitySeedConsts
  {
    public static readonly string[] CommonScopes = new[]
    {
                "email",
                "openid",
                "profile",
                "role",
                "phone",
                "address",
                "VPortal"
        };

    public static readonly string[] CommonApiUserClaims = new[]
    {
                "email",
                "email_verified",
                "name",
                "phone_number",
                "phone_number_verified",
                "role"
        };

    public static readonly string[] TenantSubdomains = new[]
    {
                "dev",
                "qa"
        };
  }
}
