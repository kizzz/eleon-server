using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Helpers.Module;
public static class HmacHelper
{
  public static string ComputeHmacSha256(string message, string secret)
  {
    var keyBytes = Encoding.UTF8.GetBytes(secret);
    var messageBytes = Encoding.UTF8.GetBytes(message);

    using (var hmac = new HMACSHA256(keyBytes))
    {
      var hashBytes = hmac.ComputeHash(messageBytes);
      return Convert.ToHexString(hashBytes);
    }
  }
}
