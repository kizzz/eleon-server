using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.SystemLog.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101407 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcSystemLogs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LogLevel = table.Column<int>(type: "int", nullable: false),
            ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            InitiatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            InitiatorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsArchived = table.Column<bool>(type: "bit", nullable: false),
            Count = table.Column<int>(type: "int", nullable: false),
            Hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcSystemLogs", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcSystemLogs");
    }
  }
}
