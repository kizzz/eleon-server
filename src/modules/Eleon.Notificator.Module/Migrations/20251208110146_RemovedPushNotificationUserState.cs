using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Notificator.Module.Migrations
{
  /// <inheritdoc />
  public partial class RemovedPushNotificationUserState : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcPushNotificationUserStates");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcPushNotificationUserStates",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            LastAckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastAckNotificationLog = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcPushNotificationUserStates", x => x.Id);
          });
    }
  }
}
