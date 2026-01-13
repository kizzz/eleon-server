using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Helpers.Module;
public static class ApiKeySignatureHelper
{
  public const string NonceGuidFormat = "N";
  public const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
  public const string Method = "POST";
  public const string Path = "/auth/connect/token";
  public const string Salt = $"{Method}:{Path}";


  private static string StrigifyTimestamp(DateTime timestamp)
  {
    return timestamp.ToString("yyyy-MM-dd'T'HH.mm.ss.fff'Z'", CultureInfo.InvariantCulture);
  }

  private static string StrigifyTimestampToUnix(DateTime timestamp)
  {
    return ((DateTimeOffset)timestamp).ToUnixTimeSeconds().ToString();
  }

  public static (string Nonce, string Timestamp, string Signature) GenerateSignature(string secretKey)
  {
    var nonce = Guid.NewGuid().ToString(NonceGuidFormat);

    var now = DateTime.UtcNow;

    var timestamp = StrigifyTimestamp(now);

    var rawMessage = $"{timestamp}:{nonce}:{Salt}";

    // HMAC-SHA256 signature
    var signature = HmacHelper.ComputeHmacSha256(rawMessage, secretKey);
    return (nonce, now.ToString(TimestampFormat), signature);
  }

  public static string GenerateSignature(string secretKey, string nonce, DateTime timestampDateTime)
  {
    var timestamp = StrigifyTimestamp(timestampDateTime);

    var rawMessage = $"{timestamp}:{nonce}:{Salt}";

    // HMAC-SHA256 signature
    var signature = HmacHelper.ComputeHmacSha256(rawMessage, secretKey);

    return signature;
  }
}
