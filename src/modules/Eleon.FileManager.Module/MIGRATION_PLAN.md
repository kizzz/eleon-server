# FileManager Module Migration Plan: Unify File and Folder Entities

## Overview

This document outlines the migration strategy for consolidating `FileEntity` and `VirtualFolderEntity` into a single `FileSystemEntry` entity with Table Per Hierarchy (TPH) mapping in EF Core.

## Current State

- **Tables**: `Files` and `VirtualFolders` (separate tables)
- **Entities**: `FileEntity` and `VirtualFolderEntity` (separate entities)

## Target State

- **Table**: `FileSystemEntries` (single table with TPH)
- **Entity**: `FileSystemEntry` (unified entity with `EntryKind` discriminator)
- **Discriminator**: `EntryKind` enum (File = 0, Folder = 1)

## Migration Strategy

### Option 1: Staged Migration (Recommended for Production)

#### Phase 1: Add New Table (Zero Downtime)
1. Create new `FileSystemEntries` table with TPH structure
2. Add `EntryKind` discriminator column
3. Include all columns from both `Files` and `VirtualFolders` tables
4. Make file-specific columns nullable (Extension, Path, ThumbnailPath, etc.)
5. Make folder-specific columns nullable (PhysicalFolderId, IsShared)

#### Phase 2: Data Migration
1. Migrate all `Files` records to `FileSystemEntries` with `EntryKind = 0` (File)
2. Migrate all `VirtualFolders` records to `FileSystemEntries` with `EntryKind = 1` (Folder)
3. Map `FolderId` from `Files` to `ParentId` in unified table
4. Preserve all audit fields (CreationTime, LastModificationTime, etc.)
5. Update foreign key references in related tables:
   - `FileStatuses` (FileId references)
   - `FileExternalLinks` (FileId references)
   - `FileArchiveFavourites` (FileId/FolderId references)

#### Phase 3: Verification
1. Verify record counts match: `Files.Count + VirtualFolders.Count == FileSystemEntries.Count`
2. Verify all foreign key relationships are intact
3. Run application tests to ensure functionality
4. Monitor for any data inconsistencies

#### Phase 4: Drop Old Tables
1. Drop foreign key constraints referencing old tables
2. Drop `Files` table
3. Drop `VirtualFolders` table
4. Update any remaining references

### Option 2: Direct Migration (For Development/Testing)

1. Backup database
2. Create `FileSystemEntries` table
3. Migrate data in single transaction
4. Update foreign keys
5. Drop old tables
6. Verify and test

## SQL Migration Scripts

### Create New Table

```sql
CREATE TABLE [EcFileSystemEntries] (
    [Id] nvarchar(450) NOT NULL,
    [EntryKind] int NOT NULL,
    [ArchiveId] uniqueidentifier NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    [ParentId] nvarchar(450) NULL,
    [FolderId] nvarchar(450) NULL, -- Legacy mapping
    [Size] nvarchar(50) NULL,
    [TenantId] uniqueidentifier NULL,
    
    -- File-specific (nullable)
    [Extension] nvarchar(50) NULL,
    [Path] nvarchar(2000) NULL,
    [ThumbnailPath] nvarchar(2000) NULL,
    
    -- Folder-specific (nullable)
    [PhysicalFolderId] nvarchar(450) NULL,
    [IsShared] bit NOT NULL DEFAULT 0,
    
    -- Audit fields
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [DeletionTime] datetime2 NULL,
    [DeleterId] uniqueidentifier NULL,
    
    CONSTRAINT [PK_EcFileSystemEntries] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_EcFileSystemEntries_ParentId] ON [EcFileSystemEntries] ([ParentId]);
CREATE INDEX [IX_EcFileSystemEntries_ArchiveId] ON [EcFileSystemEntries] ([ArchiveId]);
CREATE INDEX [IX_EcFileSystemEntries_EntryKind] ON [EcFileSystemEntries] ([EntryKind]);
CREATE INDEX [IX_EcFileSystemEntries_ParentId_Name_EntryKind] ON [EcFileSystemEntries] ([ParentId], [Name], [EntryKind]);
```

### Data Migration Script

```sql
-- Migrate Files
INSERT INTO [EcFileSystemEntries] (
    [Id], [EntryKind], [ArchiveId], [Name], [ParentId], [FolderId],
    [Extension], [Path], [Size], [ThumbnailPath], [TenantId],
    [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId],
    [IsDeleted], [DeletionTime], [DeleterId]
)
SELECT 
    [Id], 0 AS [EntryKind], [ArchiveId], [Name], [ParentId], [FolderId],
    [Extension], [Path], [Size], [ThumbnailPath], [TenantId],
    [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId],
    [IsDeleted], [DeletionTime], [DeleterId]
FROM [EcFiles];

-- Migrate VirtualFolders
INSERT INTO [EcFileSystemEntries] (
    [Id], [EntryKind], [ArchiveId], [Name], [ParentId],
    [Size], [PhysicalFolderId], [IsShared], [TenantId],
    [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId],
    [IsDeleted], [DeletionTime], [DeleterId]
)
SELECT 
    [Id], 1 AS [EntryKind], 
    -- ArchiveId: May need to determine from context or set to default
    COALESCE([ArchiveId], '00000000-0000-0000-0000-000000000000') AS [ArchiveId],
    [Name], [ParentId],
    [Size], [PhysicalFolderId], [IsShared], [TenantId],
    [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId],
    [IsDeleted], [DeletionTime], [DeleterId]
FROM [EcVirtualFolders];
```

### Update Foreign Key References

```sql
-- Note: Update any foreign key tables that reference Files or VirtualFolders
-- Example for FileStatuses (if FileId references need updating):
-- This depends on your specific schema
```

### Drop Old Tables

```sql
-- Drop foreign key constraints first
-- ALTER TABLE [EcFileStatuses] DROP CONSTRAINT [FK_...];
-- (Repeat for all foreign key constraints)

-- Drop old tables
DROP TABLE [EcFiles];
DROP TABLE [EcVirtualFolders];
```

## Rollback Strategy

If migration fails:

1. **Keep new table**: Do not drop `FileSystemEntries` immediately
2. **Restore old tables**: Restore from backup if dropped
3. **Revert code**: Deploy previous version of application code
4. **Data verification**: Verify data integrity in old tables
5. **Investigate**: Identify root cause of migration failure

## Testing Checklist

- [ ] Unit tests pass for `FileSystemEntryDomainService`
- [ ] Integration tests pass for repository operations
- [ ] API endpoints return correct data
- [ ] File operations (upload, download, move, rename) work
- [ ] Folder operations (create, delete, move, rename) work
- [ ] Hierarchy navigation works correctly
- [ ] Permissions and status filtering work
- [ ] Search functionality works for both files and folders
- [ ] Foreign key relationships are preserved

## Notes

- The `ArchiveId` field may need special handling for folders if they don't currently have it
- The `FolderId` field is kept for legacy compatibility but maps to `ParentId`
- All file-specific fields are nullable to support folder entries
- All folder-specific fields are nullable to support file entries
- The discriminator `EntryKind` is required and non-nullable

## EF Core Migration Commands

If using EF Core migrations:

```bash
# Add migration
dotnet ef migrations add UnifyFileAndFolderEntities --project Eleon.FileManager.Module.EntityFrameworkCore --startup-project <HostProject>

# Review migration
# Edit migration if needed to use staged approach

# Apply migration (staged approach)
# Apply Phase 1 migration first
dotnet ef database update --project Eleon.FileManager.Module.EntityFrameworkCore --startup-project <HostProject>

# Run data migration script manually
# Then apply Phase 2 migration to drop old tables
```

## Post-Migration Tasks

1. Update any remaining code references to old entities
2. Update documentation
3. Monitor application logs for errors
4. Verify performance (indexes should help)
5. Consider removing legacy `FolderId` field in future version



