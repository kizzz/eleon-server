using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EleonS3.HttpApi.SigV4;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace EleonS3.Test.Tests;

public class SigV4ValidatorTests
{
    [Fact]
    public void Validate_WithValidSignature_ReturnsTrue()
    {
        var ctx = new DefaultHttpContext();
        var req = ctx.Request;
        req.Method = "GET";
        req.Path = "/s3/test-bucket/path";
        req.QueryString = new QueryString("?a=1&b=2");
        req.Headers["host"] = "example.com";
        req.Headers["x-amz-date"] = "20240101T000000Z";
        req.Headers["x-amz-content-sha256"] = "UNSIGNED-PAYLOAD";

        var signedHeaders = "host;x-amz-content-sha256;x-amz-date";
        var secret = "secretKey";
        var date = "20240101";
        var region = "us-east-1";
        var service = "s3";

        var signature = ComputeSignature(req, secret, date, region, service, signedHeaders);

        var ok = SigV4Validator.Validate(req, secret, date, region, service, signedHeaders, signature, out var payloadHash);

        Assert.True(ok);
        Assert.Equal("UNSIGNED-PAYLOAD", payloadHash);
    }

    [Fact]
    public void Validate_WithInvalidSignature_ReturnsFalse()
    {
        var ctx = new DefaultHttpContext();
        var req = ctx.Request;
        req.Method = "GET";
        req.Path = "/s3/test-bucket/path";
        req.QueryString = new QueryString("?a=1");
        req.Headers["host"] = "example.com";
        req.Headers["x-amz-date"] = "20240101T000000Z";
        req.Headers["x-amz-content-sha256"] = "UNSIGNED-PAYLOAD";

        var ok = SigV4Validator.Validate(req, "secretKey", "20240101", "us-east-1", "s3", "host;x-amz-content-sha256;x-amz-date", new string('b', 64), out _);

        Assert.False(ok);
    }

    [Fact]
    public void Validate_MissingAmzDate_Throws()
    {
        var ctx = new DefaultHttpContext();
        var req = ctx.Request;
        req.Method = "GET";
        req.Path = "/s3/test-bucket/path";
        req.Headers["host"] = "example.com";

        Assert.Throws<InvalidOperationException>(() =>
            SigV4Validator.Validate(req, "secretKey", "20240101", "us-east-1", "s3", "host", new string('a', 64), out _));
    }

    [Fact]
    public void Validate_MissingPayloadHash_UsesUnsignedPayload()
    {
        var ctx = new DefaultHttpContext();
        var req = ctx.Request;
        req.Method = "GET";
        req.Path = "/s3/test-bucket/path";
        req.QueryString = new QueryString("?a=1");
        req.Headers["host"] = "example.com";
        req.Headers["x-amz-date"] = "20240101T000000Z";

        var signedHeaders = "host;x-amz-date";
        var signature = ComputeSignature(req, "secretKey", "20240101", "us-east-1", "s3", signedHeaders);

        var ok = SigV4Validator.Validate(req, "secretKey", "20240101", "us-east-1", "s3", signedHeaders, signature, out var payloadHash);

        Assert.True(ok);
        Assert.Equal("UNSIGNED-PAYLOAD", payloadHash);
    }

    private static string ComputeSignature(HttpRequest req, string accessKeySecret, string credentialScopeDate, string credentialScopeRegion, string credentialScopeService, string signedHeadersCsv)
    {
        var (canonHeaders, signedHeadersList) = CanonicalizeHeaders(req, signedHeadersCsv);
        var canonQuery = CanonicalizeQueryString(req);
        var canonUri = CanonicalizePath(req.Path.Value ?? "/");
        var payloadHashHex = req.Headers.TryGetValue("x-amz-content-sha256", out var hv) ? hv.ToString() : "UNSIGNED-PAYLOAD";
        if (string.IsNullOrEmpty(payloadHashHex)) payloadHashHex = "UNSIGNED-PAYLOAD";

        var canonicalRequest = $"{req.Method}\n{canonUri}\n{canonQuery}\n{canonHeaders}\n{signedHeadersList}\n{payloadHashHex}";
        var canonicalRequestHash = ToHex(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalRequest)));

        var amzDate = req.Headers["x-amz-date"].ToString();
        var scope = $"{credentialScopeDate}/{credentialScopeRegion}/{credentialScopeService}/aws4_request";
        var stringToSign = $"AWS4-HMAC-SHA256\n{amzDate}\n{scope}\n{canonicalRequestHash}";

        var kDate = HmacSHA256(Encoding.UTF8.GetBytes("AWS4" + accessKeySecret), credentialScopeDate);
        var kRegion = HmacSHA256(kDate, credentialScopeRegion);
        var kService = HmacSHA256(kRegion, credentialScopeService);
        var kSigning = HmacSHA256(kService, "aws4_request");

        return ToHex(HmacSHA256(kSigning, stringToSign));
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
        var parsed = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.QueryString.Value ?? "");
        var items = new List<(string, string)>();
        foreach (var kv in parsed)
        {
            foreach (var v in kv.Value)
            {
                items.Add((Uri.EscapeDataString(kv.Key), Uri.EscapeDataString(v)));
            }
        }
        items.Sort((a, b) => a.Item1 != b.Item1 ? string.CompareOrdinal(a.Item1, b.Item1) : string.CompareOrdinal(a.Item2, b.Item2));
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
}
