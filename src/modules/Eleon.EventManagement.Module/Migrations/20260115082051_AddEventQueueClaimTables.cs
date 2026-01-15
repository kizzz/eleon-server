using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.EventManagement.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddEventQueueClaimTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EcEventQueueMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QueueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lane = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    EnqueueSeq = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LockedUntilUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    VisibleAfterUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    MessageKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcEventQueueMessages", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "EcEventQueueMessageBodies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Payload = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, defaultValue: "application/json"),
                    Encoding = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcEventQueueMessageBodies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcEventQueueMessageBodies_EcEventQueueMessages_Id",
                        column: x => x.Id,
                        principalTable: "EcEventQueueMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EcEventQueueMessages_QueueId_Lane_MessageKey",
                table: "EcEventQueueMessages",
                columns: new[] { "QueueId", "Lane", "MessageKey" },
                unique: true,
                filter: "[MessageKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EcEventQueueMessages_QueueId_Lane_Status_EnqueueSeq",
                table: "EcEventQueueMessages",
                columns: new[] { "QueueId", "Lane", "Status", "EnqueueSeq" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_EcEventQueueMessages_QueueId_Lane_Status_LockedUntilUtc",
                table: "EcEventQueueMessages",
                columns: new[] { "QueueId", "Lane", "Status", "LockedUntilUtc" })
                .Annotation("SqlServer:Include", new[] { "Id", "EnqueueSeq" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcEventQueueMessageBodies");

            migrationBuilder.DropTable(
                name: "EcEventQueueMessages");
        }
    }
}
