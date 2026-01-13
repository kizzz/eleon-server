using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.Storage.Module.EntityFrameworkCore;

public class StorageDbContextFactory : DefaultDbContextFactoryBase<StorageDbContext>
{
  protected override StorageDbContext CreateDbContext(
      DbContextOptions<StorageDbContext> dbContextOptions)
  {
    return new StorageDbContext(dbContextOptions);
  }
}
