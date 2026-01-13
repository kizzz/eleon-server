using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.FileManager.Module.Migrations
{
  /// <inheritdoc />
  public partial class MergedFileAndFolders : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcFiles");

      migrationBuilder.DropTable(
          name: "EcVirtualFolders");

      migrationBuilder.CreateTable(
          name: "EcFileSystemEntries",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            ParentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
            EntryKind = table.Column<int>(type: "int", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            Extension = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            Path = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            ThumbnailPath = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            FolderId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
            PhysicalFolderId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
            IsShared = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcFileSystemEntries", x => x.Id);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcFileSystemEntries_ArchiveId",
          table: "EcFileSystemEntries",
          column: "ArchiveId");

      migrationBuilder.CreateIndex(
          name: "IX_EcFileSystemEntries_EntryKind",
          table: "EcFileSystemEntries",
          column: "EntryKind");

      migrationBuilder.CreateIndex(
          name: "IX_EcFileSystemEntries_ParentId",
          table: "EcFileSystemEntries",
          column: "ParentId");

      migrationBuilder.CreateIndex(
          name: "IX_EcFileSystemEntries_ParentId_Name_EntryKind",
          table: "EcFileSystemEntries",
          columns: new[] { "ParentId", "Name", "EntryKind" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcFileSystemEntries");

      migrationBuilder.CreateTable(
          name: "EcFiles",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            FolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ThumbnailPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFiles", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcVirtualFolders",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            IsShared = table.Column<bool>(type: "bit", nullable: false),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhysicalFolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcVirtualFolders", x => x.Id);
          });
    }
  }
}
