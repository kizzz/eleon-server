using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharedModule.modules.AppSettings.Module;
using Volo.Abp.Data;
using VPortal;

namespace EleonsoftSdk.modules.Migration.Module;

public abstract class DefaultDbContextFactoryBase<TDbContext>
  : IDesignTimeDbContextFactory<TDbContext>
  where TDbContext : DbContext
{
  public static IConfiguration _configuration = null;

  public static DbContextOptions<TDbContext> CreateOptions()
  {
    if (_configuration == null)
    {
      _configuration = SettingsLoader.LoadConfiguration();
    }

    // todo get ConnectionStringName attribute and its value or default
    var connectionStringName = typeof(TDbContext)
      .GetCustomAttributes(typeof(ConnectionStringNameAttribute), inherit: true)
      .Cast<ConnectionStringNameAttribute>()
      .FirstOrDefault()
      ?.Name;

    if (string.IsNullOrEmpty(connectionStringName))
    {
      connectionStringName = "Default";
    }

    var builder = new DbContextOptionsBuilder<TDbContext>().UseSqlServer(
      _configuration.GetConnectionString(connectionStringName),
      opt =>
      {
        opt.UseCompatibilityLevel(
          _configuration.GetValue("SqlServer:CompatibilityLevel", 120)
        );
        opt.EnableRetryOnFailure(
          maxRetryCount: 5,
          maxRetryDelay: TimeSpan.FromSeconds(30),
          errorNumbersToAdd: null
        );
      }
    );
    return builder.Options;
  }

  public TDbContext CreateDbContext(string[] args)
  {
    StartupArgsParser.SetArgs(args);

    return CreateDbContext(CreateOptions());
  }

  protected abstract TDbContext CreateDbContext(DbContextOptions<TDbContext> dbContextOptions);
}
