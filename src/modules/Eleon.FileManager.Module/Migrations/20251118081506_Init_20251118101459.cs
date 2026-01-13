using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.FileManager.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101459 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcFileArchiveFavourites",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFileArchiveFavourites", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFileArchivePermissions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ActorType = table.Column<int>(type: "int", nullable: false),
            ActorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFileArchivePermissions", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFileArchives",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FileArchiveHierarchyType = table.Column<int>(type: "int", nullable: false),
            StorageProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FileEditStorageProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            RootFolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcFileArchives", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFileExternalLinks",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PermissionType = table.Column<int>(type: "int", nullable: false),
            ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExternalFileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcFileExternalLinks", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFiles",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ThumbnailPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcFiles", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFileStatuses",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FileStatus = table.Column<int>(type: "int", nullable: false),
            StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
            table.PrimaryKey("PK_EcFileStatuses", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcPhysicalFolders",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SystemFolderName = table.Column<long>(type: "bigint", nullable: false),
            ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcPhysicalFolders", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcVirtualFolders",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhysicalFolderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsShared = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcVirtualFolders", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFileArchivePermissionTypeEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PermissionType = table.Column<int>(type: "int", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            FileArchivePermissionEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFileArchivePermissionTypeEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcFileArchivePermissionTypeEntity_EcFileArchivePermissions_FileArchivePermissionEntityId",
                      column: x => x.FileArchivePermissionEntityId,
                      principalTable: "EcFileArchivePermissions",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcFileExternalLinkReviewerEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ExpirationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ReviewerType = table.Column<int>(type: "int", nullable: false),
            ReviewerKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LastReviewDates = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExternalLinkCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ReviewerStatus = table.Column<int>(type: "int", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            FileExternalLinkEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFileExternalLinkReviewerEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcFileExternalLinkReviewerEntity_EcFileExternalLinks_FileExternalLinkEntityId",
                      column: x => x.FileExternalLinkEntityId,
                      principalTable: "EcFileExternalLinks",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcFileArchivePermissionTypeEntity_FileArchivePermissionEntityId",
          table: "EcFileArchivePermissionTypeEntity",
          column: "FileArchivePermissionEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcFileExternalLinkReviewerEntity_FileExternalLinkEntityId",
          table: "EcFileExternalLinkReviewerEntity",
          column: "FileExternalLinkEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcFileArchiveFavourites");

      migrationBuilder.DropTable(
          name: "EcFileArchivePermissionTypeEntity");

      migrationBuilder.DropTable(
          name: "EcFileArchives");

      migrationBuilder.DropTable(
          name: "EcFileExternalLinkReviewerEntity");

      migrationBuilder.DropTable(
          name: "EcFiles");

      migrationBuilder.DropTable(
          name: "EcFileStatuses");

      migrationBuilder.DropTable(
          name: "EcPhysicalFolders");

      migrationBuilder.DropTable(
          name: "EcVirtualFolders");

      migrationBuilder.DropTable(
          name: "EcFileArchivePermissions");

      migrationBuilder.DropTable(
          name: "EcFileExternalLinks");
    }
  }
}
