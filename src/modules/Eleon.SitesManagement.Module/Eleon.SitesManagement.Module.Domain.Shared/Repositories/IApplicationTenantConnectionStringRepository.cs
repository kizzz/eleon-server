using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.SitesManagement.Module.Entities;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Shared.Repositories;
public interface IApplicationTenantConnectionStringRepository : IBasicRepository<ApplicationTenantConnectionStringEntity, Guid>
{
}

