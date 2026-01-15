using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Notificator.Module.Migrations
{
    /// <inheritdoc />
    public partial class RenamedDataParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataParams",
                table: "EcNotificationLogs",
                newName: "LanguageKeyParams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LanguageKeyParams",
                table: "EcNotificationLogs",
                newName: "DataParams");
        }
    }
}
