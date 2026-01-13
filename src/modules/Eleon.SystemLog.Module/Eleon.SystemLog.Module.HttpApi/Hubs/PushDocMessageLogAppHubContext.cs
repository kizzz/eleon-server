//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Threading.Tasks;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using VPortal.DocMessageLog.Module.DocMessageLogs;
//using VPortal.DocMessageLog.Module.Entities;
//using Logging.Module;

//namespace VPortal.DocMessageLog.Module.Hubs
//{
//    [ExposeServices(typeof(IPushDocMessageLogClient))]
//    public class PushDocMessageLogAppHubContext : IPushDocMessageLogClient
//    {
//        private readonly IVportalLogger<PushDocMessageLogAppHubContext> _logger;
//        private readonly ICurrentTenant _currentTenant;
//        private readonly IHubContext<PushDocMessageLogHub, IPushDocMessageLogClient> _hubContext;

//        public PushDocMessageLogAppHubContext(
//            IVportalLogger<PushDocMessageLogAppHubContext> logger,
//            ICurrentTenant currentTenant,
//            IHubContext<PushDocMessageLogHub, IPushDocMessageLogClient> hubContext)
//        {
//            _logger = logger;
//            _currentTenant = currentTenant;
//            _hubContext = hubContext;
//        }

//        [AllowAnonymous]
//        public async Task PushLog(DocMessageLogDto logMsg)
//        {
//            try
//            {
//                var group = _hubContext.Clients.Group(new DocMessageLogTenantGroupId(_currentTenant).ToString());
//                if (group != null)
//                {
//                    await group.PushLog(logMsg);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.Capture(ex);
//            }
//            finally
//            {
//            }
//        }

//        [AllowAnonymous]
//        public async Task RetryLog(DocMessageLogDto logMsg)
//        {
//            try
//            {
//                var group = _hubContext.Clients.Group(new DocMessageLogTenantGroupId(_currentTenant).ToString());
//                if (group != null)
//                {
//                    await group.RetryLog(logMsg);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.Capture(ex);
//            }
//            finally
//            {
//            }
//        }

//        [AllowAnonymous]
//        public Task RemoveLog(Guid logId)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
