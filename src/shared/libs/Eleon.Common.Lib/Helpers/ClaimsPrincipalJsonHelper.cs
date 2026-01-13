using IdentityModel;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace Eleon.Common.Lib.Helpers;

public static class ClaimsPrincipalJsonHelper
{
    // SINGLE source of truth for JSON behavior
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    // -------------------------
    // Serialize
    // -------------------------
    public static string Serialize(ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var identity = principal.Identity as ClaimsIdentity;

        var payload = new
        {
            authenticationType = identity?.AuthenticationType,
            nameClaimType = identity?.NameClaimType ?? ClaimTypes.Name,
            roleClaimType = identity?.RoleClaimType ?? ClaimTypes.Role,

            claims = principal.Claims.Select(c => new
            {
                type = c.Type,
                value = c.Value,
                valueType = c.ValueType,
                issuer = c.Issuer,
                originalIssuer = c.OriginalIssuer
            })
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    // -------------------------
    // Deserialize
    // -------------------------
    public static ClaimsPrincipal Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON is null or empty", nameof(json));

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var authenticationType = GetString(root, "authenticationType");
        var nameClaimType = GetString(root, "nameClaimType") ?? ClaimTypes.Name;
        var roleClaimType = GetString(root, "roleClaimType") ?? ClaimTypes.Role;

        var claims = new List<Claim>();

        if (root.TryGetProperty("claims", out var claimsEl))
        {
            foreach (var c in claimsEl.EnumerateArray())
            {
                var type = GetString(c, "type") ?? throw new InvalidOperationException("Claim type missing");
                var value = GetString(c, "value") ?? "";

                var valueType = GetString(c, "valueType") ?? ClaimValueTypes.String;
                var issuer = GetString(c, "issuer") ?? ClaimsIdentity.DefaultIssuer;
                var originalIssuer = GetString(c, "originalIssuer") ?? issuer;

                if (type == JwtClaimTypes.Subject)
                {
                    claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", value, valueType, issuer, originalIssuer));
                }
                else if (type == JwtClaimTypes.Role)
                {
                    claims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", value, valueType, issuer, originalIssuer));
                }
            }
        }

        var identity = new ClaimsIdentity(
            claims,
            authenticationType,
            nameClaimType,
            roleClaimType
        );

        return new ClaimsPrincipal(identity);
    }

    // -------------------------
    // Helpers
    // -------------------------
    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop)
            ? prop.GetString()
            : null;
    }
}
