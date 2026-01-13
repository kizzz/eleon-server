
using System.Text.RegularExpressions;

namespace EleonCore.Modules.S3.HttpApi.S3.SigV4;

public sealed record SigV4AuthParts(
    string AccessKeyId,
    string CredentialScopeDate,
    string CredentialScopeRegion,
    string CredentialScopeService,
    string SignedHeadersCsv,
    string SignatureHex
);

public static class SigV4Parser
{
    private static readonly Regex AuthRx = new Regex(
        @"^AWS4-HMAC-SHA256\s+Credential=(?<akid>[^/]+)/(?<date>\d{8})/(?<region>[^/]+)/(?<service>[^/]+)/aws4_request,\s*SignedHeaders=(?<signed>[^,]+),\s*Signature=(?<sig>[0-9a-fA-F]{64})$",
        RegexOptions.Compiled);

    public static SigV4AuthParts Parse(string authorizationHeader)
    {
        var m = AuthRx.Match(authorizationHeader?.Trim() ?? string.Empty);
        if (!m.Success) throw new InvalidOperationException("Invalid AWS SigV4 Authorization header");

        return new SigV4AuthParts(
            m.Groups["akid"].Value,
            m.Groups["date"].Value,
            m.Groups["region"].Value,
            m.Groups["service"].Value,
            m.Groups["signed"].Value,
            m.Groups["sig"].Value.ToLowerInvariant()
        );
    }
}
