using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Helpers.Module;
public static class DatabaseCheckHelper
{
  public static int MaxRetries = 3;
  public static int DelayMilliseconds = 60000; // 1 minute

  public static async Task<bool> IsDatabaseAvailableAsync(string connectionString)
  {
    ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
    
    // Use a shorter timeout for startup checks to prevent hanging
    const int StartupTimeoutSeconds = 5;
    
    try
    {
      // Try with a short timeout first for startup
      var ct = new CancellationTokenSource(TimeSpan.FromSeconds(StartupTimeoutSeconds));
      using var connection = new SqlConnection(connectionString);
      await connection.OpenAsync(ct.Token);
      return true; // Connection successful
    }
    catch (Exception ex)
    {
      // Log but don't throw - allow startup to continue even if DB is temporarily unavailable
      // The application can retry later when actually needed
      try
      {
        Log.Logger?.Warning($"Database check failed during startup (will retry later): {ex.Message}");
      }
      catch
      {
        // Silently ignore if logging fails (Serilog might not be ready)
      }
      return false; // Return false instead of throwing to allow startup to continue
    }
  }

  public static async Task<bool> CheckDefaultDbConnection(IConfiguration configuration)
  {
    if (configuration != null)
    {
      var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren().Select(c => c.Value);

      foreach (var connectionString in connectionStrings)
      {
        if (!string.IsNullOrEmpty(connectionString))
        {
          // Don't block startup - just check quickly and continue
          var result = await DatabaseCheckHelper.IsDatabaseAvailableAsync(connectionString);
          // Return true if at least one connection works, but don't fail startup if none work
          if (result)
            return true;
        }
      }
    }

    // Return false but don't throw - allow startup to continue
    return false;
  }
}
