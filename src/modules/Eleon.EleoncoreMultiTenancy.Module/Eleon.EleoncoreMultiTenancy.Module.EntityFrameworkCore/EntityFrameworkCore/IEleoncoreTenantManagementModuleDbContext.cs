using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public interface IEleoncoreTenantManagementModuleDbContext : IEfCoreDbContext
{
}
