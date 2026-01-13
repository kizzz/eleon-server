using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.Host.Exceptions
{
    internal class GatewayHostLicenseException : Exception
    {
        private const string LicenseMsg = "An error occured while checking license";

        public GatewayHostLicenseException(string? message) : base($"{LicenseMsg}:  {message}")
        {
        }
    }
}
