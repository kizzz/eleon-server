using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.SystemServicesModule.Module.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public interface ISystemServicesModuleDbContext : IEfCoreDbContext
{
}

