using System.Diagnostics;

namespace VPortal.ProxyClient.Host
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            int resultCode = await ProxyClientHost.ConfigureAndRun(args);
            return resultCode;
        }
    }
}