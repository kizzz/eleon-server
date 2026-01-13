using System.Collections.Immutable;
using System.Text;

namespace Eleon.Mcp.Infrastructure.Utilities;

public static class CommandLineTokenizer
{
    public static IReadOnlyList<string> Tokenize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        var tokens = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < raw.Length; i++)
        {
            var c = raw[i];
            if (c == '"' && (i == 0 || raw[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                CommitToken();
                continue;
            }

            if (c == '\\' && i + 1 < raw.Length)
            {
                var next = raw[i + 1];
                if (next is '"' or '\\')
                {
                    current.Append(next);
                    i++;
                    continue;
                }
            }

            current.Append(c);
        }

        CommitToken();
        return tokens.ToImmutableArray();

        void CommitToken()
        {
            if (current.Length == 0)
            {
                return;
            }

            tokens.Add(current.ToString());
            current.Clear();
        }
    }
}

