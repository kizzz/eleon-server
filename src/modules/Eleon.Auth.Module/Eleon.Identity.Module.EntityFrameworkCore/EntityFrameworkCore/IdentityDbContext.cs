using Microsoft.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using System.Reflection.Emit;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module.EntityFrameworkCore;

[ConnectionStringName(IdentityDbProperties.ConnectionStringName)]
public class IdentityDbContext : AbpDbContext<IdentityDbContext>, IIdentityDbContext
{
  public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    // builder.ConfigureEleoncoreIdentity();
    builder.ConfigureIdentityServer();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
