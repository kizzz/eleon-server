using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.JobScheduler.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101535 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcTasks",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CanRunManually = table.Column<bool>(type: "bit", nullable: false),
            RestartAfterFailInterval = table.Column<TimeSpan>(type: "time", nullable: true),
            RestartAfterFailMaxAttempts = table.Column<int>(type: "int", nullable: false),
            CurrentRetryAttempt = table.Column<int>(type: "int", nullable: false),
            Timeout = table.Column<TimeSpan>(type: "time", nullable: true),
            AllowForceStop = table.Column<bool>(type: "bit", nullable: false),
            LastRunTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            NextRunTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcTasks", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcActions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ActionParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ActionExtraParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            RetryInterval = table.Column<TimeSpan>(type: "time", nullable: true),
            MaxRetryAttempts = table.Column<int>(type: "int", nullable: false),
            TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            MaxDelayInMinutes = table.Column<int>(type: "int", nullable: false),
            OnFailureRecepients = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcActions", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcActions_EcTasks_TaskId",
                      column: x => x.TaskId,
                      principalTable: "EcTasks",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcTaskExecutions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            RunnedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            RunnedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            RunnedByTriggerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            RunnedByTriggerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            FinishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
            table.PrimaryKey("PK_EcTaskExecutions", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcTaskExecutions_EcTasks_TaskId",
                      column: x => x.TaskId,
                      principalTable: "EcTasks",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcTriggers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            LastRun = table.Column<DateTime>(type: "datetime2", nullable: true),
            StartUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExpireUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            PeriodType = table.Column<int>(type: "int", nullable: false),
            Period = table.Column<int>(type: "int", nullable: false),
            DaysOfWeek = table.Column<int>(type: "int", nullable: false),
            DaysOfWeekOccurences = table.Column<int>(type: "int", nullable: false),
            DaysOfMonth = table.Column<long>(type: "bigint", nullable: false),
            Months = table.Column<int>(type: "int", nullable: false),
            RepeatTask = table.Column<bool>(type: "bit", nullable: false),
            RepeatIntervalUnits = table.Column<int>(type: "int", nullable: false),
            RepeatIntervalUnitType = table.Column<int>(type: "int", nullable: false),
            RepeatDurationUnits = table.Column<int>(type: "int", nullable: true),
            RepeatDurationUnitType = table.Column<int>(type: "int", nullable: false),
            TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
            table.PrimaryKey("PK_EcTriggers", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcTriggers_EcTasks_TaskId",
                      column: x => x.TaskId,
                      principalTable: "EcTasks",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcActionParentEntity",
          columns: table => new
          {
            ChildActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ParentActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcActionParentEntity", x => new { x.ChildActionId, x.ParentActionId });
            table.ForeignKey(
                      name: "FK_EcActionParentEntity_EcActions_ChildActionId",
                      column: x => x.ChildActionId,
                      principalTable: "EcActions",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_EcActionParentEntity_EcActions_ParentActionId",
                      column: x => x.ParentActionId,
                      principalTable: "EcActions",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "EcActionExecutionEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            TaskExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ActionParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ActionExtraParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            StatusChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsStatusChangedManually = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcActionExecutionEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcActionExecutionEntity_EcActions_ActionId",
                      column: x => x.ActionId,
                      principalTable: "EcActions",
                      principalColumn: "Id");
            table.ForeignKey(
                      name: "FK_EcActionExecutionEntity_EcTaskExecutions_TaskExecutionId",
                      column: x => x.TaskExecutionId,
                      principalTable: "EcTaskExecutions",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcActionExecutionParentEntity",
          columns: table => new
          {
            ChildActionExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ParentActionExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcActionExecutionParentEntity", x => new { x.ChildActionExecutionId, x.ParentActionExecutionId });
            table.ForeignKey(
                      name: "FK_EcActionExecutionParentEntity_EcActionExecutionEntity_ChildActionExecutionId",
                      column: x => x.ChildActionExecutionId,
                      principalTable: "EcActionExecutionEntity",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_EcActionExecutionParentEntity_EcActionExecutionEntity_ParentActionExecutionId",
                      column: x => x.ParentActionExecutionId,
                      principalTable: "EcActionExecutionEntity",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcActionExecutionEntity_ActionId",
          table: "EcActionExecutionEntity",
          column: "ActionId");

      migrationBuilder.CreateIndex(
          name: "IX_EcActionExecutionEntity_TaskExecutionId",
          table: "EcActionExecutionEntity",
          column: "TaskExecutionId");

      migrationBuilder.CreateIndex(
          name: "IX_EcActionExecutionParentEntity_ParentActionExecutionId",
          table: "EcActionExecutionParentEntity",
          column: "ParentActionExecutionId");

      migrationBuilder.CreateIndex(
          name: "IX_EcActionParentEntity_ParentActionId",
          table: "EcActionParentEntity",
          column: "ParentActionId");

      migrationBuilder.CreateIndex(
          name: "IX_EcActions_TaskId",
          table: "EcActions",
          column: "TaskId");

      migrationBuilder.CreateIndex(
          name: "IX_EcTaskExecutions_TaskId",
          table: "EcTaskExecutions",
          column: "TaskId");

      migrationBuilder.CreateIndex(
          name: "IX_EcTriggers_TaskId",
          table: "EcTriggers",
          column: "TaskId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcActionExecutionParentEntity");

      migrationBuilder.DropTable(
          name: "EcActionParentEntity");

      migrationBuilder.DropTable(
          name: "EcTriggers");

      migrationBuilder.DropTable(
          name: "EcActionExecutionEntity");

      migrationBuilder.DropTable(
          name: "EcActions");

      migrationBuilder.DropTable(
          name: "EcTaskExecutions");

      migrationBuilder.DropTable(
          name: "EcTasks");
    }
  }
}
