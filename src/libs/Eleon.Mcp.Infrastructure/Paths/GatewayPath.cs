namespace Eleon.Mcp.Infrastructure.Paths;

public static class GatewayPath
{
    public static string Normalize(string? raw)
    {
        var value = string.IsNullOrWhiteSpace(raw) ? "/sse" : raw.Trim();
        if (!value.StartsWith('/'))
        {
            value = "/" + value;
        }

        if (value.Length > 1 && value.EndsWith('/'))
        {
            value = value.TrimEnd('/');
        }

        return value;
    }
}

