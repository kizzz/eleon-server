using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text.RegularExpressions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.ProxyClient.Config;
using VPortal.ProxyClient.Domain.Shared.Constants;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using VPortal.ProxyClient.Domain.Shared.Status;
using VPortal.ProxyClient.Domain.Settings;
using VPortal.ProxyClient.Domain.Auth;
using Logging.Module;

namespace VPortal.ProxyClient.Domain.Status
{
    [UnitOfWork]
    public class ProxyClientStatusDomainService : DomainService, ISingletonDependency
    {
        private readonly IVportalLogger<ProxyClientStatusDomainService> logger;
        private readonly ProxyClientSettingsDomainService settingsDomainService;
        private readonly ProxyAuthDomainService authDomainService;

        public ProxyClientStatusDomainService(
            IVportalLogger<ProxyClientStatusDomainService> logger,
            ProxyClientSettingsDomainService settingsDomainService,
            ProxyAuthDomainService authDomainService) 
        {
            this.logger = logger;
            this.settingsDomainService = settingsDomainService;
            this.authDomainService = authDomainService;
        }

        public async Task<ProxyStatusInformation> GetProxyStatus()
        {
            var port = settingsDomainService.GetConfiguredPort();
            var currentRegStage = await authDomainService.GetCurrentRegistrationStage();
            return new ProxyStatusInformation(currentRegStage, (int)port);
        }
    }
}
