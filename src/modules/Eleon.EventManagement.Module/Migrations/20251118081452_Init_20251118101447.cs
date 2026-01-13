using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.EventManagement.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101447 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcEventManagementQueueDefenition",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            MessagesLimit = table.Column<int>(type: "int", nullable: false),
            Messages = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcEventManagementQueueDefenition", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcEventManagementQueues",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            QueueDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Forwarding = table.Column<string>(type: "nvarchar(max)", nullable: true),
            MessagesLimit = table.Column<int>(type: "int", nullable: false),
            Count = table.Column<int>(type: "int", nullable: false),
            Head = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Tail = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcEventManagementQueues", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcEventManagementEvents",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
            QueueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Next = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Prev = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcEventManagementEvents", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcEventManagementEvents_EcEventManagementQueues_QueueId",
                      column: x => x.QueueId,
                      principalTable: "EcEventManagementQueues",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcEventManagementEvents_QueueId",
          table: "EcEventManagementEvents",
          column: "QueueId");

      migrationBuilder.CreateIndex(
          name: "IX_EcEventManagementQueues_CreationTime",
          table: "EcEventManagementQueues",
          column: "CreationTime");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcEventManagementEvents");

      migrationBuilder.DropTable(
          name: "EcEventManagementQueueDefenition");

      migrationBuilder.DropTable(
          name: "EcEventManagementQueues");
    }
  }
}
