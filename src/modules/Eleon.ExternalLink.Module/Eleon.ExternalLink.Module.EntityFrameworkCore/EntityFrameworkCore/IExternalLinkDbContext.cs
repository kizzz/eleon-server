using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.ExternalLink.Module.Entities;

namespace VPortal.ExternalLink.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface IExternalLinkDbContext : IEfCoreDbContext
{
  public DbSet<ExternalLinkEntity> ExternalLinks { get; set; }
}
