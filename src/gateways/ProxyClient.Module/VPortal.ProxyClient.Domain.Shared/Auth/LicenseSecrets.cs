using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VPortal.ProxyClient.Domain.Shared.Auth
{
    [Serializable]
    [XmlRoot("LicenseSecrets")]
    public class LicenseSecrets
    {
        public string ClientCompoundKey { get; set; }
        public string CurrentRegistrationStage { get; set; }
        public string MachineKey { get; set; }
        public string Certificate { get; set; }
    }
}
