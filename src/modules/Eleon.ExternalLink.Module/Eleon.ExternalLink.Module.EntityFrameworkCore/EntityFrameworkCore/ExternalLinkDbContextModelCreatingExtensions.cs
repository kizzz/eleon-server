using Common.Module.Extensions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace VPortal.ExternalLink.Module.EntityFrameworkCore;

public static class ExternalLinkDbContextModelCreatingExtensions
{
  public static void ConfigureModule(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

  }
}
