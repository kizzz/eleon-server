using EleonCore.Modules.S3.HttpApi.S3.SigV4;
using EleonS3.Domain;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Volo.Abp.MultiTenancy;

namespace EleonS3.HttpApi.SigV4;

public class SigV4AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _tenant;
    private readonly SigV4Manager _sigV4Manager;

    public SigV4AuthMiddleware(RequestDelegate next, ICurrentTenant tenant, SigV4Manager sigV4Manager, IConfiguration configuration)
    {
        _next = next;
        _tenant = tenant;
        _sigV4Manager = sigV4Manager;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!ctx.Request.Path.StartsWithSegments("/s3", StringComparison.OrdinalIgnoreCase))
        {
            await _next(ctx);
            return;
        }

        if (_tenant.Id != null)
        {
            await _next(ctx); return;
        }

        var auth = ctx.Request.Headers["Authorization"].ToString();
        if (auth?.StartsWith("AWS4-HMAC-SHA256") == true)
        {
            try
            {
                var parts = SigV4Parser.Parse(auth);
                var accessKey = _configuration.GetSection("SigV4:AccessKey").Value;
                var secretKey = _configuration.GetSection("SigV4:SecretKey").Value ?? throw new Exception("SecretKey not found in configuration");
                if (accessKey != parts.AccessKeyId)
                {
                    ctx.Response.StatusCode = 403;
                    _sigV4Manager.SigV4AuthorizationSucceded = false;
                    await _next(ctx);
                    return;
                }

                var ok = SigV4Validator.Validate(ctx.Request, secretKey,
                    parts.CredentialScopeDate, parts.CredentialScopeRegion, parts.CredentialScopeService,
                    parts.SignedHeadersCsv, parts.SignatureHex, out _);

                if (!ok)
                {
                    ctx.Response.StatusCode = 401;
                    _sigV4Manager.SigV4AuthorizationSucceded = false;
                    await _next(ctx);
                    return;
                }

                _sigV4Manager.SigV4AuthorizationSucceded = true;
                await _next(ctx);
                return;

            }
            catch
            {
                ctx.Response.StatusCode = 401;
                _sigV4Manager.SigV4AuthorizationSucceded = false;
                return;
            }
        }
        else
        {
            await _next(ctx);
        }
    }
}
