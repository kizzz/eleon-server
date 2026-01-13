using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public interface IEventManagementModuleDbContext : IEfCoreDbContext
{
}
