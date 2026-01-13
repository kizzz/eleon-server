using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifecycleModule.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101553 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcConditions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ConditionTargetType = table.Column<int>(type: "int", nullable: false),
            ConditionType = table.Column<int>(type: "int", nullable: false),
            ConditionResultType = table.Column<int>(type: "int", nullable: false),
            RefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcConditions", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcStatesGroupAudits",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DocumentObjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StatesGroupTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DocumentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            CurrentStateOrderIndex = table.Column<int>(type: "int", nullable: true),
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
            table.PrimaryKey("PK_EcStatesGroupAudits", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcStatesGroupTemplates",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            DocumentObjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStatesGroupTemplates", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcRuleEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FunctionType = table.Column<int>(type: "int", nullable: false),
            Function = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ConditionEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcRuleEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcRuleEntity_EcConditions_ConditionEntityId",
                      column: x => x.ConditionEntityId,
                      principalTable: "EcConditions",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcStateAuditEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            StatesGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StatesGroupAuditId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            StatesTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            OrderIndex = table.Column<int>(type: "int", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            StateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsMandatory = table.Column<bool>(type: "bit", nullable: false),
            IsReadOnly = table.Column<bool>(type: "bit", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            ApprovalType = table.Column<int>(type: "int", nullable: false),
            CurrentActorOrderIndex = table.Column<int>(type: "int", nullable: true),
            LastStatusDate = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStateAuditEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcStateAuditEntity_EcStatesGroupAudits_StatesGroupAuditId",
                      column: x => x.StatesGroupAuditId,
                      principalTable: "EcStatesGroupAudits",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcStateTemplateEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            StatesGroupTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            OrderIndex = table.Column<int>(type: "int", nullable: false),
            StateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsMandatory = table.Column<bool>(type: "bit", nullable: false),
            IsReadOnly = table.Column<bool>(type: "bit", nullable: false),
            ApprovalType = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStateTemplateEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcStateTemplateEntity_EcStatesGroupTemplates_StatesGroupTemplateId",
                      column: x => x.StatesGroupTemplateId,
                      principalTable: "EcStatesGroupTemplates",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcStateActorAuditEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ActorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StateActorTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            OrderIndex = table.Column<int>(type: "int", nullable: true),
            IsConditional = table.Column<bool>(type: "bit", nullable: false),
            RuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsApprovalNeeded = table.Column<bool>(type: "bit", nullable: false),
            IsFormAdmin = table.Column<bool>(type: "bit", nullable: false),
            IsApprovalManager = table.Column<bool>(type: "bit", nullable: false),
            IsApprovalAdmin = table.Column<bool>(type: "bit", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            ActorType = table.Column<int>(type: "int", nullable: false),
            RefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StatusUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StatusUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStateActorAuditEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcStateActorAuditEntity_EcStateAuditEntity_StateId",
                      column: x => x.StateId,
                      principalTable: "EcStateAuditEntity",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcStateActorTemplateEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ActorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StateTemplateEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            StateTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            OrderIndex = table.Column<int>(type: "int", nullable: true),
            RefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ActorType = table.Column<int>(type: "int", nullable: false),
            IsConditional = table.Column<bool>(type: "bit", nullable: false),
            RuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsApprovalNeeded = table.Column<bool>(type: "bit", nullable: false),
            IsFormAdmin = table.Column<bool>(type: "bit", nullable: false),
            IsApprovalManager = table.Column<bool>(type: "bit", nullable: false),
            IsApprovalAdmin = table.Column<bool>(type: "bit", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStateActorTemplateEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcStateActorTemplateEntity_EcStateTemplateEntity_StateTemplateEntityId",
                      column: x => x.StateTemplateEntityId,
                      principalTable: "EcStateTemplateEntity",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcStateActorTaskListSettingTemplateEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            DocumentObjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TaskListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StateActorTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStateActorTaskListSettingTemplateEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcStateActorTaskListSettingTemplateEntity_EcStateActorTemplateEntity_StateActorTemplateId",
                      column: x => x.StateActorTemplateId,
                      principalTable: "EcStateActorTemplateEntity",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcRuleEntity_ConditionEntityId",
          table: "EcRuleEntity",
          column: "ConditionEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStateActorAuditEntity_StateId",
          table: "EcStateActorAuditEntity",
          column: "StateId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStateActorTaskListSettingTemplateEntity_StateActorTemplateId",
          table: "EcStateActorTaskListSettingTemplateEntity",
          column: "StateActorTemplateId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStateActorTemplateEntity_StateTemplateEntityId",
          table: "EcStateActorTemplateEntity",
          column: "StateTemplateEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStateAuditEntity_StatesGroupAuditId",
          table: "EcStateAuditEntity",
          column: "StatesGroupAuditId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStateTemplateEntity_StatesGroupTemplateId",
          table: "EcStateTemplateEntity",
          column: "StatesGroupTemplateId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcRuleEntity");

      migrationBuilder.DropTable(
          name: "EcStateActorAuditEntity");

      migrationBuilder.DropTable(
          name: "EcStateActorTaskListSettingTemplateEntity");

      migrationBuilder.DropTable(
          name: "EcConditions");

      migrationBuilder.DropTable(
          name: "EcStateAuditEntity");

      migrationBuilder.DropTable(
          name: "EcStateActorTemplateEntity");

      migrationBuilder.DropTable(
          name: "EcStatesGroupAudits");

      migrationBuilder.DropTable(
          name: "EcStateTemplateEntity");

      migrationBuilder.DropTable(
          name: "EcStatesGroupTemplates");
    }
  }
}
