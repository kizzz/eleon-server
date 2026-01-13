using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace VPortal.ApplicationConfiguration.Module.EntityFrameworkCore;

/// <summary>
/// Example model configuration for ApplicationConfiguration module
/// </summary>
public static class ApplicationConfigurationDbContextModelCreatingExtensions
{
  public static void ConfigureApplicationConfiguration(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));


  }
}
