using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.LanguageManagement.Module.EntityFrameworkCore;

namespace VPortal.LanguageManagement.Module.Module.EntityFrameworkCore;

public class LanguageManagementDbContextFactory : DefaultDbContextFactoryBase<LanguageManagementDbContext>
{
  protected override LanguageManagementDbContext CreateDbContext(
      DbContextOptions<LanguageManagementDbContext> dbContextOptions)
  {
    return new LanguageManagementDbContext(dbContextOptions);
  }
}
