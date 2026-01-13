using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.HealthCheck.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101516 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcHealthCheckReports",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ServiceVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UpTime = table.Column<long>(type: "bigint", nullable: false),
            CheckName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsPublic = table.Column<bool>(type: "bit", nullable: false),
            HealthCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcHealthCheckReports", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcHealthChecks",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            InitiatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            InProgress = table.Column<bool>(type: "bit", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcHealthChecks", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcReportExtraInformation",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Severity = table.Column<int>(type: "int", nullable: false),
            Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            HealthCheckReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcReportExtraInformation", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcReportExtraInformation_EcHealthCheckReports_HealthCheckReportId",
                      column: x => x.HealthCheckReportId,
                      principalTable: "EcHealthCheckReports",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcHealthCheckReports_HealthCheckId",
          table: "EcHealthCheckReports",
          column: "HealthCheckId");

      migrationBuilder.CreateIndex(
          name: "IX_EcReportExtraInformation_HealthCheckReportId",
          table: "EcReportExtraInformation",
          column: "HealthCheckReportId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcHealthChecks");

      migrationBuilder.DropTable(
          name: "EcReportExtraInformation");

      migrationBuilder.DropTable(
          name: "EcHealthCheckReports");
    }
  }
}
