using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Notificator.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedIsReadFieldToNotificationLog : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<bool>(
          name: "IsRead",
          table: "EcNotificationLogs",
          type: "bit",
          nullable: false,
          defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "IsRead",
          table: "EcNotificationLogs");
    }
  }
}
