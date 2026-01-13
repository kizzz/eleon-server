using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.ExternalLink.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101452 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcExternalLinks",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ExpirationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            PublicParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PrivateParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LoginType = table.Column<int>(type: "int", nullable: false),
            DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LoginKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LoginAttempts = table.Column<int>(type: "int", nullable: false),
            LastLoginSuccessDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastLoginAttemptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastPublicRequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            IsOneTimeLink = table.Column<bool>(type: "bit", nullable: false),
            ExternalLinkCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExternalLinkUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcExternalLinks", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcExternalLinks");
    }
  }
}
