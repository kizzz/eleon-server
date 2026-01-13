using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.ProxyClient.Host.Cli.Helpers;

namespace VPortal.ProxyClient.Host.Cli.Cli
{
    public class CliExceptionHandler
    {
        public static async Task ExecuteWithHandling(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLineError(ex.Message);
                Environment.Exit(0);
            }
        }
    }
}
