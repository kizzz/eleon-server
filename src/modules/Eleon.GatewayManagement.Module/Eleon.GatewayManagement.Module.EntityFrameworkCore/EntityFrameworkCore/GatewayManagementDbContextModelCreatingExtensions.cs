using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace VPortal.GatewayManagement.Module.EntityFrameworkCore;

public static class GatewayManagementDbContextModelCreatingExtensions
{
  public static void ConfigureGatewayManagement(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

  }

  private static bool IsAbpTable(string tableName)
      => tableName.StartsWith("Abp") || tableName == "ExtraPropertyDictionary";
}
