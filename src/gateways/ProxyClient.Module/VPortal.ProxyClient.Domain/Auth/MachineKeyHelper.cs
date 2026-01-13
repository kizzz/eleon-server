using System.Security.Cryptography;
using System.Text;
using Common.Module.Helpers;

namespace VPortal.ProxyClient.Domain.Auth
{
    public class MachineKeyHelper
    {
        private static string rawMachineKeyCache;
        public static string GetMachineKeyHash()
        {
            if (rawMachineKeyCache == null)
            {
                string sid = MachineSpecificsHelper.GetComputerSid();
                var secretBytes = Encoding.UTF8.GetBytes(sid);
                using (var sha = SHA256.Create())
                {
                    var secretHash = sha.ComputeHash(secretBytes);
                    rawMachineKeyCache = Convert.ToHexString(secretHash);
                }
            }

            return rawMachineKeyCache;
        }
    }
}
