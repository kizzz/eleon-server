using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Accounting.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddedNameFieldToPackageTemplateModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EcPackageTemplateModuleEntity",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "EcPackageTemplateModuleEntity");
        }
    }
}
