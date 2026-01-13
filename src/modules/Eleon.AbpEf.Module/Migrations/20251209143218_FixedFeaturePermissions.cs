using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Abp.Module.Migrations
{
  /// <inheritdoc />
  public partial class FixedFeaturePermissions : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropIndex(
          name: "IX_EcPermissions_Name",
          table: "EcPermissions");

      migrationBuilder.DropIndex(
          name: "IX_EcPermissionGroups_Name",
          table: "EcPermissionGroups");

      migrationBuilder.DropIndex(
          name: "IX_EcFeatures_Name",
          table: "EcFeatures");

      migrationBuilder.DropIndex(
          name: "IX_EcFeatureGroups_Name",
          table: "EcFeatureGroups");

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissions_Name",
          table: "EcPermissions",
          column: "Name");

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissionGroups_Name",
          table: "EcPermissionGroups",
          column: "Name");

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatures_Name",
          table: "EcFeatures",
          column: "Name");

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatureGroups_Name",
          table: "EcFeatureGroups",
          column: "Name");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropIndex(
          name: "IX_EcPermissions_Name",
          table: "EcPermissions");

      migrationBuilder.DropIndex(
          name: "IX_EcPermissionGroups_Name",
          table: "EcPermissionGroups");

      migrationBuilder.DropIndex(
          name: "IX_EcFeatures_Name",
          table: "EcFeatures");

      migrationBuilder.DropIndex(
          name: "IX_EcFeatureGroups_Name",
          table: "EcFeatureGroups");

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissions_Name",
          table: "EcPermissions",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissionGroups_Name",
          table: "EcPermissionGroups",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatures_Name",
          table: "EcFeatures",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatureGroups_Name",
          table: "EcFeatureGroups",
          column: "Name",
          unique: true);
    }
  }
}
