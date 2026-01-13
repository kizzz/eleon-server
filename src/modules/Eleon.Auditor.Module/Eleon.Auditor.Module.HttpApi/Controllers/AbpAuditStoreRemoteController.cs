//using Logging.Module;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using Volo.Abp;
//using Volo.Abp.Auditing;
//using VPortal.Auditor.Module.Remote;

//namespace VPortal.Auditor.Module.Controllers
//{
//    [Area("ProxyClient")]
//    [RemoteService(Name = ModuleRemoteServiceConsts.ProxyRemoteServiceName)]
//    [Route("api/Auditor/RemoteStore")]
//    public class AbpAuditStoreRemoteController : ModuleController, IAbpAuditStoreRemoteAppService
//    {
//        private readonly IAbpAuditStoreRemoteAppService appService;
//        private readonly IVportalLogger<AbpAuditStoreRemoteController> _logger;

//        public AbpAuditStoreRemoteController(
//            IAbpAuditStoreRemoteAppService appService,
//            IVportalLogger<AbpAuditStoreRemoteController> logger)
//        {
//            this.appService = appService;
//            _logger = logger;
//        }

//        [DisableAuditing]
//        [HttpPost("SaveAuditLogInfo")]
//        public async Task<bool> SaveAuditLogInfo(AuditLogInfo auditInfo)
//        {

//            var response = await appService.SaveAuditLogInfo(auditInfo);


//            return response;
//        }
//    }
//}
