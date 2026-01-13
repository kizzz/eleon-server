using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Notificator.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101601 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcNotificationLogs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Priority = table.Column<int>(type: "int", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsLocalizedData = table.Column<bool>(type: "bit", nullable: false),
            DataParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsRedirectEnabled = table.Column<bool>(type: "bit", nullable: false),
            RedirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcNotificationLogs", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcNotifications",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            BackgroundJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Receivers = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SerializedType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Priority = table.Column<int>(type: "int", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            EnvironmentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcNotifications", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcPushNotificationUserStates",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            LastAckNotificationLog = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastAckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcPushNotificationUserStates", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUserNotificationSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SourceType = table.Column<int>(type: "int", nullable: false),
            SendNative = table.Column<bool>(type: "bit", nullable: false),
            SendEmail = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserNotificationSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcWebPushes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
            P256Dh = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Auth = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcWebPushes", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcNotificationLogs");

      migrationBuilder.DropTable(
          name: "EcNotifications");

      migrationBuilder.DropTable(
          name: "EcPushNotificationUserStates");

      migrationBuilder.DropTable(
          name: "EcUserNotificationSettings");

      migrationBuilder.DropTable(
          name: "EcWebPushes");
    }
  }
}
