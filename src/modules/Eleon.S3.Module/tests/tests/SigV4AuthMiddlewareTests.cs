using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonS3.Domain;
using EleonS3.HttpApi.SigV4;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace EleonS3.Test.Tests;

public class SigV4AuthMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_NonS3Path_CallsNext()
    {
        var called = false;
        var middleware = BuildMiddleware(ctx =>
        {
            called = true;
            return Task.CompletedTask;
        });

        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/health";

        await middleware.InvokeAsync(ctx);

        Assert.True(called);
    }

    [Fact]
    public async Task InvokeAsync_TenantPresent_CallsNext()
    {
        var called = false;
        var tenant = TestMockHelpers.CreateMockCurrentTenant(Guid.NewGuid(), "tenant");
        var middleware = BuildMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        }, tenant: tenant);

        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/s3/bucket";

        await middleware.InvokeAsync(ctx);

        Assert.True(called);
    }

    [Fact]
    public async Task InvokeAsync_InvalidAccessKey_Sets403()
    {
        var called = false;
        var manager = new SigV4Manager();
        var middleware = BuildMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        }, manager);

        var ctx = BuildSignedContext("INVALID", "secretKey", "20240101", "us-east-1", "s3");

        await middleware.InvokeAsync(ctx);

        Assert.Equal(StatusCodes.Status403Forbidden, ctx.Response.StatusCode);
        Assert.False(manager.SigV4AuthorizationSucceded);
        Assert.True(called);
    }

    [Fact]
    public async Task InvokeAsync_InvalidSignature_Sets401()
    {
        var called = false;
        var manager = new SigV4Manager();
        var middleware = BuildMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        }, manager);

        var ctx = BuildSignedContext("AKID", "secretKey", "20240101", "us-east-1", "s3");
        ctx.Request.Headers["Authorization"] = ctx.Request.Headers["Authorization"].ToString().Replace(ctx.Request.Headers["Authorization"].ToString().Split("Signature=")[1], new string('b', 64));

        await middleware.InvokeAsync(ctx);

        Assert.Equal(StatusCodes.Status401Unauthorized, ctx.Response.StatusCode);
        Assert.False(manager.SigV4AuthorizationSucceded);
        Assert.True(called);
    }

    [Fact]
    public async Task InvokeAsync_ValidSignature_SetsFlag()
    {
        var called = false;
        var manager = new SigV4Manager();
        var middleware = BuildMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        }, manager);

        var ctx = BuildSignedContext("AKID", "secretKey", "20240101", "us-east-1", "s3");

        await middleware.InvokeAsync(ctx);

        Assert.True(manager.SigV4AuthorizationSucceded);
        Assert.True(called);
    }

    private static SigV4AuthMiddleware BuildMiddleware(RequestDelegate next, SigV4Manager? manager = null, ICurrentTenant? tenant = null)
    {
        manager ??= new SigV4Manager();
        tenant ??= TestMockHelpers.CreateMockCurrentTenant(null, "host");
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SigV4:AccessKey"] = "AKID",
                ["SigV4:SecretKey"] = "secretKey"
            })
            .Build();

        return new SigV4AuthMiddleware(next, tenant, manager, config);
    }

    private static DefaultHttpContext BuildSignedContext(string accessKey, string secretKey, string date, string region, string service)
    {
        var ctx = new DefaultHttpContext();
        var req = ctx.Request;
        req.Method = "GET";
        req.Path = "/s3/bucket";
        req.QueryString = new QueryString("?a=1");
        req.Headers["host"] = "example.com";
        req.Headers["x-amz-date"] = date + "T000000Z";
        req.Headers["x-amz-content-sha256"] = "UNSIGNED-PAYLOAD";

        var signedHeaders = "host;x-amz-content-sha256;x-amz-date";
        var signature = ComputeSignature(req, secretKey, date, region, service, signedHeaders);

        req.Headers["Authorization"] = $"AWS4-HMAC-SHA256 Credential={accessKey}/{date}/{region}/{service}/aws4_request, SignedHeaders={signedHeaders}, Signature={signature}";
        return ctx;
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
