//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Volo.Abp.MultiTenancy;

//namespace VPortal.Auditor.Module.Hubs
//{
//    internal class AuditorTenantGroupId
//    {
//        private readonly ICurrentTenant tenant;

//        public AuditorTenantGroupId(ICurrentTenant tenant)
//        {
//            this.tenant = tenant;
//        }

//        public override string ToString() => $"T-{tenant.Id?.ToString() ?? "HOST"}";
//    }
//}
