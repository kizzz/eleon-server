using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.FileManager.Module.EntityFrameworkCore
{
  public class FileManagerDbContextFactory : DefaultDbContextFactoryBase<FileManagerDbContext>
  {
    protected override FileManagerDbContext CreateDbContext(
        DbContextOptions<FileManagerDbContext> dbContextOptions)
    {
      return new FileManagerDbContext(dbContextOptions);
    }
  }
}
