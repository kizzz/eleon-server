using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.DocMessageLog.Module.Entities;

namespace VPortal.DocMessageLog.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface ISystemLogDbContext : IEfCoreDbContext
{
  public DbSet<SystemLogEntity> SystemLogs { get; set; }
}
