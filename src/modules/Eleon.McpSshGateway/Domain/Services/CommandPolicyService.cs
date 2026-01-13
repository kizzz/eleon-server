using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using Eleon.McpSshGateway.Domain.Entities;

namespace Eleon.McpSshGateway.Domain.Services;

public sealed class CommandPolicyService
{
    private readonly ConcurrentDictionary<string, Regex> regexCache = new(StringComparer.OrdinalIgnoreCase);

    public bool IsAllowed(SshHost host, string command)
    {
        ArgumentNullException.ThrowIfNull(host);

        if (!host.IsEnabled)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(command))
        {
            return false;
        }

        if (MatchesAny(host.DenyPatterns, command))
        {
            return false;
        }

        if (host.AllowPatterns.Count == 0)
        {
            return false;
        }

        return MatchesAny(host.AllowPatterns, command);
    }

    private bool MatchesAny(IEnumerable<string> patterns, string command)
    {
        foreach (var pattern in patterns)
        {
            if (GetRegex(pattern).IsMatch(command))
            {
                return true;
            }
        }

        return false;
    }

    private Regex GetRegex(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            pattern = "*";
        }

        return regexCache.GetOrAdd(pattern, static key =>
        {
            var regexPattern = WildcardToRegex(key);
            return new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        });
    }

    private static string WildcardToRegex(string pattern)
    {
        var builder = new StringBuilder(pattern.Length * 2);
        builder.Append('^');
        foreach (var ch in pattern)
        {
            builder.Append(ch switch
            {
                '*' => ".*",
                '?' => ".",
                _ => Regex.Escape(ch.ToString())
            });
        }

        builder.Append('$');
        return builder.ToString();
    }
}
