using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.SitesManagement.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118105755 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcApplicationTenantConnectionStrings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcApplicationTenantConnectionStrings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcClientApplicationMenuItems",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsUrl = table.Column<bool>(type: "bit", nullable: false),
            IsNewWindow = table.Column<bool>(type: "bit", nullable: false),
            Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ParentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Order = table.Column<int>(type: "int", nullable: false),
            RequiredPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            MenuType = table.Column<int>(type: "int", nullable: false),
            ItemType = table.Column<int>(type: "int", nullable: false),
            Display = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcClientApplicationMenuItems", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcClientApplicationModules",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            Type = table.Column<int>(type: "int", nullable: false),
            IsDefault = table.Column<bool>(type: "bit", nullable: false),
            Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsHealthCheckEnabled = table.Column<bool>(type: "bit", nullable: false),
            LastHealthCheckStatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            HealthCheckStatusMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
            HealthCheckStatus = table.Column<int>(type: "int", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcClientApplicationModules", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcClientApplications",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            FrameworkType = table.Column<int>(type: "int", nullable: false),
            StyleType = table.Column<int>(type: "int", nullable: false),
            ClientApplicationType = table.Column<int>(type: "int", nullable: false),
            ErrorHandlingLevel = table.Column<int>(type: "int", nullable: false),
            Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsDefault = table.Column<bool>(type: "bit", nullable: false),
            HeadString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UseDedicatedDatabase = table.Column<bool>(type: "bit", nullable: false),
            IsAuthenticationRequired = table.Column<bool>(type: "bit", nullable: false),
            RequiredPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AppType = table.Column<int>(type: "int", nullable: false),
            ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Expose = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LoadLevel = table.Column<int>(type: "int", nullable: false),
            OrderIndex = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcClientApplications", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcClientApplicationModulesRelations",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ClientApplicationEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PluginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            OrderIndex = table.Column<int>(type: "int", nullable: false),
            Expose = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LoadLevel = table.Column<int>(type: "int", nullable: false),
            ApplicationEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcClientApplicationModulesRelations", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcClientApplicationModulesRelations_EcClientApplications_ApplicationEntityId",
                      column: x => x.ApplicationEntityId,
                      principalTable: "EcClientApplications",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcClientApplicationPropertyEntities",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ApplicationEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ApplicationModuleEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcClientApplicationPropertyEntities", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcClientApplicationPropertyEntities_EcClientApplicationModulesRelations_ApplicationModuleEntityId",
                      column: x => x.ApplicationModuleEntityId,
                      principalTable: "EcClientApplicationModulesRelations",
                      principalColumn: "Id");
            table.ForeignKey(
                      name: "FK_EcClientApplicationPropertyEntities_EcClientApplications_ApplicationEntityId",
                      column: x => x.ApplicationEntityId,
                      principalTable: "EcClientApplications",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcClientApplicationModulesRelations_ApplicationEntityId",
          table: "EcClientApplicationModulesRelations",
          column: "ApplicationEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcClientApplicationPropertyEntities_ApplicationEntityId",
          table: "EcClientApplicationPropertyEntities",
          column: "ApplicationEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcClientApplicationPropertyEntities_ApplicationModuleEntityId",
          table: "EcClientApplicationPropertyEntities",
          column: "ApplicationModuleEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcApplicationTenantConnectionStrings");

      migrationBuilder.DropTable(
          name: "EcClientApplicationMenuItems");

      migrationBuilder.DropTable(
          name: "EcClientApplicationModules");

      migrationBuilder.DropTable(
          name: "EcClientApplicationPropertyEntities");

      migrationBuilder.DropTable(
          name: "EcClientApplicationModulesRelations");

      migrationBuilder.DropTable(
          name: "EcClientApplications");
    }
  }
}
