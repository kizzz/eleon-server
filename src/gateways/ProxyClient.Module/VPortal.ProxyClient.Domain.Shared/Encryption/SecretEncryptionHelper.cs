using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.ProxyClient.Domain.Shared.Encryption
{
    internal class SecretEncryptionHelper
    {
        public static string EncryptValue(string apiKey, string value)
        {
            return value;
        }

        public static string DecryptValue(string apiKey, string encryptedValue)
        {
            return encryptedValue;
        }
    }
}
