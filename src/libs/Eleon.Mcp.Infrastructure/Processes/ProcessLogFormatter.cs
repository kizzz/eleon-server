using System.Diagnostics;
using System.Text;

namespace Eleon.Mcp.Infrastructure.Processes;

public static class ProcessLogFormatter
{
    public static string Format(ProcessStartInfo startInfo)
    {
        var builder = new StringBuilder(startInfo.FileName);
        foreach (var argument in startInfo.ArgumentList)
        {
            builder.Append(' ');
            builder.Append(argument.Contains(' ') ? $"\"{argument}\"" : argument);
        }

        return builder.ToString();
    }
}

