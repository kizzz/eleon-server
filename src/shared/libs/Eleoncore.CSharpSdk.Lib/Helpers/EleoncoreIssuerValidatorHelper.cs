using Microsoft.IdentityModel.Tokens;

namespace Eleoncore.SDK.Helpers
{
  public class EleoncoreIssuerValidatorHelper
  {
    public static List<string> Authorities { get; set; } = new List<string>
    {
    };
    public EleoncoreIssuerValidatorHelper() { }


    public static string ValidateIssuer(string issuer)
    {
      foreach (var authority in Authorities)
      {
        string validatedIssuer = ValidateIssuer(authority + "/auth", issuer);
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

  }
}
