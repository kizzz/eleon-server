//using Logging.Module;
//using System;
//using System.Threading.Tasks;
//using Volo.Abp.Auditing;
//using VPortal.Auditor.Module.Remote;

//namespace VPortal.Auditor.Module.RemoteStore
//{
//    public class RemoteAbpAuditingStore : IAuditingStore
//    {
//        private readonly IAbpAuditStoreRemoteAppService remoteAppService;
//        private readonly IVportalLogger<RemoteAbpAuditingStore> logger;

//        public RemoteAbpAuditingStore(
//            IAbpAuditStoreRemoteAppService remoteAppService,
//            IVportalLogger<RemoteAbpAuditingStore> logger)
//        {
//            this.remoteAppService = remoteAppService;
//            this.logger = logger;
//        }

//        public async Task SaveAsync(AuditLogInfo auditInfo)
//        {
//            //SaveAsyncAndForget(auditInfo);
//        }

//        private async void SaveAsyncAndForget(AuditLogInfo auditInfo)
//        {
//            try
//            {
//                await remoteAppService.SaveAuditLogInfo(auditInfo);
//            }
//            catch (Exception ex)
//            {
//                logger.Capture(ex);
//            }

//        }
//    }
//}
