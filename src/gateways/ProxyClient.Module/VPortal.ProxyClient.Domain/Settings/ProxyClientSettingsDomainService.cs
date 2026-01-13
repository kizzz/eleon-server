using Logging.Module;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.ProxyClient.Config;

namespace VPortal.ProxyClient.Domain.Settings
{
    [UnitOfWork]
    public class ProxyClientSettingsDomainService : DomainService, ISingletonDependency
    {
        private static Regex PortRegex = new (@":[0-9]{2,5}");
        private readonly IVportalLogger<ProxyClientSettingsDomainService> logger;
        private readonly IConfiguration configuration;

        public ProxyClientSettingsDomainService(
            IVportalLogger<ProxyClientSettingsDomainService> logger,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public int? GetConfiguredPort()
        {
            string configuredUrl = configuration["App:SelfUrl"];
            if (configuredUrl == null)
            {
                return null;
            }

            var match = PortRegex.Match(configuredUrl);
            if (!match.Success)
            {
                return null;
            }

            string portStr = new string(match.Value.Skip(1).ToArray());
            if (int.TryParse(portStr, out int port))
            {
                return port;
            }

            return null;
        }

        public async Task SetPort(int newPort)
        {
            if (!File.Exists(ProxyClientConfigConsts.AppsettingsPath))
            {
                throw new UserFriendlyException("The configuration file ('appsettings.json') was not found.");
            }

            var jsonText = await File.ReadAllTextAsync(ProxyClientConfigConsts.AppsettingsPath);
            var json = JObject.Parse(jsonText);

            var appSettings = json["App"];
            appSettings["SelfUrl"] = ReplacePort(appSettings["SelfUrl"].Value<string>(), newPort);

            var endpointsSettings = json["Kestrel"]?["EndPoints"];
            if (endpointsSettings != null)
            {
                foreach (var endpointCfg in endpointsSettings.Children())
                {
                    var endpointCfgValue = (endpointCfg as JProperty)?.Value;
                    if (endpointCfgValue != null)
                    {
                        var urlProp = endpointCfgValue["Url"];
                        if (urlProp != null)
                        {
                            endpointCfgValue["Url"] = ReplacePort(urlProp.Value<string>(), newPort);
                        }
                    }
                }
            }

            var updatedJson = json.ToString();
            await File.WriteAllTextAsync(ProxyClientConfigConsts.AppsettingsPath, updatedJson);
        }

        private string ReplacePort(string url, int newPort)
         => PortRegex.Replace(url, $":{newPort}", 1);
    }
}
