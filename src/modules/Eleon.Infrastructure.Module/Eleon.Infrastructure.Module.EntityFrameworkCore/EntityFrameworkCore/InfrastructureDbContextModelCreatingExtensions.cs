using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace VPortal.Infrastructure.Module.EntityFrameworkCore;

public static class InfrastructureDbContextModelCreatingExtensions
{
  public static void ConfigureModule(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));
  }
}
