using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Collaboration.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101639 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcChatMembers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            RefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Role = table.Column<int>(type: "int", nullable: false),
            Type = table.Column<int>(type: "int", nullable: false),
            LastViewedByUser = table.Column<DateTime>(type: "datetime2", nullable: true),
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
            table.PrimaryKey("PK_EcChatMembers", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcChatRooms",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            RefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Type = table.Column<int>(type: "int", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsPublic = table.Column<bool>(type: "bit", nullable: false),
            DefaultRole = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcChatRooms", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUserChatSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsChatMuted = table.Column<bool>(type: "bit", nullable: false),
            IsArchived = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcUserChatSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcChatMessages",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Sender = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SenderType = table.Column<int>(type: "int", nullable: false),
            Type = table.Column<int>(type: "int", nullable: false),
            Severity = table.Column<int>(type: "int", nullable: false),
            Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcChatMessages", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcChatMessages_EcChatRooms_ChatRoomId",
                      column: x => x.ChatRoomId,
                      principalTable: "EcChatRooms",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcViewChatPermissions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            EntityRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
            EntityType = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcViewChatPermissions", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcViewChatPermissions_EcChatRooms_ChatId",
                      column: x => x.ChatId,
                      principalTable: "EcChatRooms",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcChatMessages_ChatRoomId",
          table: "EcChatMessages",
          column: "ChatRoomId");

      migrationBuilder.CreateIndex(
          name: "IX_EcViewChatPermissions_ChatId",
          table: "EcViewChatPermissions",
          column: "ChatId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcChatMembers");

      migrationBuilder.DropTable(
          name: "EcChatMessages");

      migrationBuilder.DropTable(
          name: "EcUserChatSettings");

      migrationBuilder.DropTable(
          name: "EcViewChatPermissions");

      migrationBuilder.DropTable(
          name: "EcChatRooms");
    }
  }
}
