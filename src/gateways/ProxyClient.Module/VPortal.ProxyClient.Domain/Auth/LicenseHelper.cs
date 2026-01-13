using Microsoft.Extensions.Logging;
using NUglify.JavaScript.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Common.Module.Extensions;
using Common.Module.Helpers;
using VPortal.ProxyClient.Domain.Shared.Auth;

namespace VPortal.ProxyClient.Domain.Auth
{
    public class LicenseHelper
    {
        public const string LicenseFilename = "LIC";
        private static readonly string LicensePath = Path.Combine(AppContext.BaseDirectory, LicenseFilename);
        public static LicenseSecrets License { get; private set; }

        static LicenseHelper()
        {
            LoadLicense();
        }


        public static X509Certificate2 GetCertificate()
            => CertificateHelper.CreateCertificateFromBase64(
                    License.Certificate,
                    License.MachineKey);


        public static void ValidateLicense()
        {
            ValidateLicense(License);
        }

        public static void Save()
        {
            if (!File.Exists(LicensePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LicensePath));
            }

            if (License.MachineKey == null)
            {
                License.MachineKey = MachineKeyHelper.GetMachineKeyHash();
            }

            //string content = _xmlSerializerHelper.SerializeToXml(License);
            string content = License.SerializeToXml();
            string encrypted = EncryptionHelper.Encrypt(content);
            File.WriteAllText(LicensePath, encrypted);
        }

        private static async void LoadLicense()
        {
            try
            {
                string fileContent = File.ReadAllText(LicensePath);
                string decrypted = EncryptionHelper.Decrypt(fileContent);
                //var license = _xmlSerializerHelper.DeserializeFromXml<LicenseSecrets>(decrypted);
                var license = decrypted.DeserializeFromXml<LicenseSecrets>();
                License = license;
            }
            catch (Exception ex)
            {
                License = new LicenseSecrets();
            }
        }

        private static void ValidateLicense(LicenseSecrets license)
        {
            string machineKey = MachineKeyHelper.GetMachineKeyHash();
            if (license.MachineKey != machineKey)
            {
                throw new Exception("The license is invalid.");
            }

            if (license.ClientCompoundKey.IsNullOrEmpty())
            {
                throw new Exception("Client key not found.");
            }

            if (license.CurrentRegistrationStage.IsNullOrEmpty())
            {
                throw new Exception("License file is corrupt.");
            }
        }
    }
}
