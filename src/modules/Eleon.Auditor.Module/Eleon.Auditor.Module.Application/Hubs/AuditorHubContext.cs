//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.Identity;
//using Volo.Abp.ObjectMapping;
//using Common.Module.Constants;
//using VPortal.Infrastructure.Module.Entities;
//using Logging.Module;

//namespace VPortal.Auditor.Module.Hubs
//{
//    public class AuditorHubContext : IAuditorHubContext, ITransientDependency
//    {
//        private readonly IVportalLogger<AuditorHubContext> logger;
//        private readonly IObjectMapper objectMapper;
//        private readonly IAuditorAppHubContext auditorHub;

//        public AuditorHubContext(
//            IVportalLogger<AuditorHubContext> logger,
//            IObjectMapper objectMapper,
//            IAuditorAppHubContext auditorHub)
//        {
//            this.logger = logger;
//            this.objectMapper = objectMapper;
//            this.auditorHub = auditorHub;
//        }

//        public async Task NotifyVersionChanged(DocumentObjectType documentObjectType, string documentId, DocumentVersionEntity newVersion)
//        {
//            try
//            {
//                var dto = new AuditVersionChangeNotificationDto()
//                {
//                    DocumentObjectType = documentObjectType,
//                    NewVersion = newVersion,
//                    DocumentId = documentId,
//                };
//                await auditorHub.NotifyVersionChanged(dto);
//            }
//            catch (Exception ex)
//            {
//                logger.Capture(ex);
//            }

//        }
//    }
//}
