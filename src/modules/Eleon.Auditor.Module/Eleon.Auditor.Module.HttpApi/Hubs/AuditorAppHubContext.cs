//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Threading.Tasks;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Logging.Module;

//namespace VPortal.Auditor.Module.Hubs
//{
//    public class AuditorAppHubContext : IAuditorAppHubContext, ITransientDependency
//    {
//        private readonly IVportalLogger<AuditorAppHubContext> logger;
//        private readonly ICurrentTenant currentTenant;
//        private readonly IHubContext<AuditorHub> hubContext;

//        public AuditorAppHubContext(
//            IVportalLogger<AuditorAppHubContext> logger,
//            ICurrentTenant currentTenant,
//            IHubContext<AuditorHub> hubContext)
//        {
//            this.logger = logger;
//            this.currentTenant = currentTenant;
//            this.hubContext = hubContext;
//        }

//        [AllowAnonymous]
//        public async Task NotifyVersionChanged(AuditVersionChangeNotificationDto notification)
//        {
//            try
//            {
//                var group = hubContext.Clients.Group(new AuditorTenantGroupId(currentTenant).ToString());
//                if (group != null)
//                {
//                    await group.SendAsync("AuditorVersionChanged", notification);
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Capture(ex);
//            }

//        }
//    }
//}
