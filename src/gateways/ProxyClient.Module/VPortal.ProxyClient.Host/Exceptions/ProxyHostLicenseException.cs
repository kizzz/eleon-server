using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.ProxyClient.Host.Exceptions
{
    internal class ProxyHostLicenseException : Exception
    {
        private const string LicenseMsg = "An error occured while checking license";

        public ProxyHostLicenseException(string? message) : base($"{LicenseMsg}:  {message}")
        {
        }
    }
}
