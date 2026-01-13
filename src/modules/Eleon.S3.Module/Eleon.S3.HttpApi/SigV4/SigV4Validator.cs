
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace EleonS3.HttpApi.SigV4;

public interface ISigV4KeyStore
{
    (Guid? TenantId, string SecretKey)? Find(string accessKeyId);
}
public static class SigV4Validator
{
    public static bool Validate(HttpRequest req, string accessKeySecret, string credentialScopeDate, string credentialScopeRegion, string credentialScopeService, string signedHeadersCsv, string signatureHex, out string? payloadHashHex)
    {
        var (canonHeaders, signedHeadersList) = CanonicalizeHeaders(req, signedHeadersCsv);
        var canonQuery = CanonicalizeQueryString(req);
        var canonUri = CanonicalizePath(req.Path.Value ?? "/");
        payloadHashHex = req.Headers.TryGetValue("x-amz-content-sha256", out var hv) ? hv.ToString() : "UNSIGNED-PAYLOAD";
        if (string.IsNullOrEmpty(payloadHashHex)) payloadHashHex = "UNSIGNED-PAYLOAD";

        var canonicalRequest = $"{req.Method}\n{canonUri}\n{canonQuery}\n{canonHeaders}\n{signedHeadersList}\n{payloadHashHex}";
        var canonicalRequestHash = ToHex(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalRequest)));

        var amzDate = req.Headers["x-amz-date"].ToString();
        if (string.IsNullOrEmpty(amzDate)) throw new InvalidOperationException("Missing x-amz-date");
        var scope = $"{credentialScopeDate}/{credentialScopeRegion}/{credentialScopeService}/aws4_request";
        var stringToSign = $"AWS4-HMAC-SHA256\n{amzDate}\n{scope}\n{canonicalRequestHash}";

        var kDate = HmacSHA256(Encoding.UTF8.GetBytes("AWS4" + accessKeySecret), credentialScopeDate);
        var kRegion = HmacSHA256(kDate, credentialScopeRegion);
        var kService = HmacSHA256(kRegion, credentialScopeService);
        var kSigning = HmacSHA256(kService, "aws4_request");

        var expected = ToHex(HmacSHA256(kSigning, stringToSign));
        return FixedTimeEquals(expected, signatureHex);
    }

    private static (string canonHeaders, string signedHeadersList) CanonicalizeHeaders(HttpRequest req, string signedHeadersCsv)
    {
        var signed = signedHeadersCsv.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                     .Select(s => s.ToLowerInvariant())
                                     .OrderBy(s => s)
                                     .ToArray();
        var hdrs = new List<string>(signed.Length);
        foreach (var h in signed)
        {
            var v = req.Headers[h].ToString();
            v = string.Join(' ', v.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            hdrs.Add($"{h}:{v}");
        }
        var canon = string.Join("\n", hdrs) + "\n";
        var signedList = string.Join(";", signed);
        return (canon, signedList);
    }

    private static string CanonicalizeQueryString(HttpRequest req)
    {
        var parsed = QueryHelpers.ParseQuery(req.QueryString.Value ?? "");
        var items = new List<(string,string)>();
        foreach (var kv in parsed)
        {
            foreach (var v in kv.Value)
                items.Add((Uri.EscapeDataString(kv.Key), Uri.EscapeDataString(v)));
        }
        items.Sort((a,b) => a.Item1 != b.Item1 ? string.CompareOrdinal(a.Item1, b.Item1) : string.CompareOrdinal(a.Item2, b.Item2));
        return string.Join("&", items.Select(t => $"{t.Item1}={t.Item2}"));
    }

    private static string CanonicalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return "/";
        }

        var segments = path.Split('/', StringSplitOptions.None);
        for (var i = 0; i < segments.Length; i++)
        {
            segments[i] = Uri.EscapeDataString(segments[i]);
        }

        return string.Join("/", segments);
    }

    private static byte[] HmacSHA256(byte[] key, string data)
        => new HMACSHA256(key).ComputeHash(Encoding.UTF8.GetBytes(data));

    private static string ToHex(byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) sb.AppendFormat("{0:x2}", b);
        return sb.ToString();
    }

    private static bool FixedTimeEquals(string a, string b)
        => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
