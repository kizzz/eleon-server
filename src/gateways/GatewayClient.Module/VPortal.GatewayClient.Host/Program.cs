using System.Diagnostics;

namespace VPortal.GatewayClient.Host
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            int resultCode = await GatewayClientHost.ConfigureAndRun(args);
            return resultCode;
        }
    }
}
