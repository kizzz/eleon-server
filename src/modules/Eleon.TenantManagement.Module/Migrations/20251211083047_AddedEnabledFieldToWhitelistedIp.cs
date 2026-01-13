using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.TenantManagement.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedEnabledFieldToWhitelistedIp : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<bool>(
          name: "Enabled",
          table: "EcTenantWhitelistedIpEntity",
          type: "bit",
          nullable: false,
          defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "Enabled",
          table: "EcTenantWhitelistedIpEntity");
    }
  }
}
