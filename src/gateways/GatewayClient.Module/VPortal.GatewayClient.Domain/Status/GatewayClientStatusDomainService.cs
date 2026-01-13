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
using VPortal.GatewayClient.Config;
using VPortal.GatewayClient.Domain.Shared.Constants;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using VPortal.GatewayClient.Domain.Shared.Status;
using VPortal.GatewayClient.Domain.Settings;
using VPortal.GatewayClient.Domain.Auth;
using Logging.Module;

namespace VPortal.GatewayClient.Domain.Status
{
    [UnitOfWork]
    public class GatewayClientStatusDomainService : DomainService, ISingletonDependency
    {
        private readonly IVportalLogger<GatewayClientStatusDomainService> logger;
        private readonly GatewayClientSettingsDomainService settingsDomainService;
        private readonly GatewayAuthDomainService authDomainService;

        public GatewayClientStatusDomainService(
            IVportalLogger<GatewayClientStatusDomainService> logger,
            GatewayClientSettingsDomainService settingsDomainService,
            GatewayAuthDomainService authDomainService) 
        {
            this.logger = logger;
            this.settingsDomainService = settingsDomainService;
            this.authDomainService = authDomainService;
        }

        public async Task<GatewayStatusInformation> GetGatewayStatus()
        {
            var port = settingsDomainService.GetConfiguredPort();
            var currentRegStage = await authDomainService.GetCurrentRegistrationStage();
            return new GatewayStatusInformation(currentRegStage, (int)port);
        }
    }
}
