using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Eleoncore.SDK.Helpers
{
  public class EleonsoftAuthTokenValidationHelper
  {
    public static List<string> Authorities { get; set; } = new List<string>
    {
    };

    public static string ValidateIssuer(string issuer)
    {
      foreach (var authority in Authorities)
      {
        string validatedIssuer = ValidateIssuer(authority, issuer);
        if (validatedIssuer != null)
        {
          return validatedIssuer;
        }
        validatedIssuer = ValidateIssuer(authority + "/auth", issuer);
        if (validatedIssuer != null)
        {
          return validatedIssuer;
        }
      }

      throw new SecurityTokenInvalidIssuerException("Issuer does not match any of the valid issuers provided for this application.");
    }

    private static string ValidateIssuer(string authority, string issuer)
    {
      if (authority.EndsWith('/'))
      {
        authority = authority.Substring(0, authority.Length - 1);
      }

      var parts = authority.Split(@"//");
      bool valid = issuer.StartsWith(parts[0] + @"//") && issuer.EndsWith(parts[1]);
      if (!valid)
      {
        return null;
      }

      return issuer;
    }

    public static SecurityToken ValidateSignature(string token, TokenValidationParameters parameters)
    {
      // Parse the token using JsonWebToken or any custom logic
      var jwtSecurityToken = new JsonWebToken(token);

      // Perform any additional validation or transformation here if needed

      return jwtSecurityToken;
    }
  }
}
