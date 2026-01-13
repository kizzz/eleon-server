using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.BackgroundJobs.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101434 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcEleoncoreBackgroundJobs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ParentJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsSystemInternal = table.Column<bool>(type: "bit", nullable: false),
            SourceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            ScheduleExecutionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            LastExecutionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            IsRetryAllowed = table.Column<bool>(type: "bit", nullable: false),
            Initiator = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StartExecutionParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StartExecutionExtraParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
            JobFinishedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            EnvironmentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            MaxDelayInMinutes = table.Column<int>(type: "int", nullable: false),
            RetryIntervalInMinutes = table.Column<int>(type: "int", nullable: false),
            MaxRetryAttempts = table.Column<int>(type: "int", nullable: false),
            CurrentRetryAttempt = table.Column<int>(type: "int", nullable: false),
            NextRetryTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            OnFailureRecepients = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcEleoncoreBackgroundJobs", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcBackgroundJobExecutionEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExecutionStartTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExecutionEndTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            IsRetryExecution = table.Column<bool>(type: "bit", nullable: false),
            RetryUserInitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            StartExecutionParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StartExecutionExtraParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BackgroundJobEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StatusChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsStatusChangedManually = table.Column<bool>(type: "bit", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcBackgroundJobExecutionEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcBackgroundJobExecutionEntity_EcEleoncoreBackgroundJobs_BackgroundJobEntityId",
                      column: x => x.BackgroundJobEntityId,
                      principalTable: "EcEleoncoreBackgroundJobs",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcBackgroundJobMessageEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            TextMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
            MessageType = table.Column<int>(type: "int", nullable: false),
            BackgroundJobExecutionEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcBackgroundJobMessageEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcBackgroundJobMessageEntity_EcBackgroundJobExecutionEntity_BackgroundJobExecutionEntityId",
                      column: x => x.BackgroundJobExecutionEntityId,
                      principalTable: "EcBackgroundJobExecutionEntity",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcBackgroundJobExecutionEntity_BackgroundJobEntityId",
          table: "EcBackgroundJobExecutionEntity",
          column: "BackgroundJobEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcBackgroundJobMessageEntity_BackgroundJobExecutionEntityId",
          table: "EcBackgroundJobMessageEntity",
          column: "BackgroundJobExecutionEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcBackgroundJobMessageEntity");

      migrationBuilder.DropTable(
          name: "EcBackgroundJobExecutionEntity");

      migrationBuilder.DropTable(
          name: "EcEleoncoreBackgroundJobs");
    }
  }
}
