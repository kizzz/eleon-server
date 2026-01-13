using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Eleoncore.SDK.Helpers
{
  public class SignatureValidatorHelper
  {

    public static SecurityToken ValidateSignature(string token, TokenValidationParameters parameters)
    {
      // Parse the token using JsonWebToken or any custom logic
      var jwtSecurityToken = new JsonWebToken(token);

      // Perform any additional validation or transformation here if needed

      return jwtSecurityToken;
    }
  }
}
