using System;
using Common.Module.Constants;
using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.Tests.TestHelpers;

public class FileManagerTestDataBuilder
{
    public static FileSystemEntryBuilder FileSystemEntry() => new FileSystemEntryBuilder();
    public static FileArchiveBuilder FileArchive() => new FileArchiveBuilder();
    public static FileArchiveFavouriteBuilder FileArchiveFavourite() => new FileArchiveFavouriteBuilder();
    public static FileArchivePermissionBuilder FileArchivePermission() => new FileArchivePermissionBuilder();
    public static FileStatusBuilder FileStatus() => new FileStatusBuilder();
    public static FileExternalLinkBuilder FileExternalLink() => new FileExternalLinkBuilder();
}

public class FileSystemEntryBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private Guid _archiveId = Guid.NewGuid();
    private string _name = "TestEntry";
    private string? _parentId;
    private EntryKind _entryKind = EntryKind.File;
    private string? _extension = ".txt";
    private string? _path;
    private string? _size = "0";
    private string? _thumbnailPath;
    private string? _physicalFolderId;
    private bool _isShared = false;
    private Guid? _tenantId;

    public FileSystemEntryBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public FileSystemEntryBuilder WithArchiveId(Guid archiveId)
    {
        _archiveId = archiveId;
        return this;
    }

    public FileSystemEntryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public FileSystemEntryBuilder WithParentId(string? parentId)
    {
        _parentId = parentId;
        return this;
    }

    public FileSystemEntryBuilder AsFile()
    {
        _entryKind = EntryKind.File;
        return this;
    }

    public FileSystemEntryBuilder AsFolder()
    {
        _entryKind = EntryKind.Folder;
        return this;
    }

    public FileSystemEntryBuilder WithExtension(string? extension)
    {
        _extension = extension;
        return this;
    }

    public FileSystemEntryBuilder WithPath(string? path)
    {
        _path = path;
        return this;
    }

    public FileSystemEntryBuilder WithSize(string? size)
    {
        _size = size;
        return this;
    }

    public FileSystemEntryBuilder WithThumbnailPath(string? thumbnailPath)
    {
        _thumbnailPath = thumbnailPath;
        return this;
    }

    public FileSystemEntryBuilder WithPhysicalFolderId(string? physicalFolderId)
    {
        _physicalFolderId = physicalFolderId;
        return this;
    }

    public FileSystemEntryBuilder WithIsShared(bool isShared)
    {
        _isShared = isShared;
        return this;
    }

    public FileSystemEntryBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public FileSystemEntry Build()
    {
        FileSystemEntry entry;
        if (_entryKind == EntryKind.File)
        {
            entry = FileSystemEntry.CreateFile(_id, _archiveId, _name, _parentId, _extension, _path, _size, _thumbnailPath);
        }
        else
        {
            entry = FileSystemEntry.CreateFolder(_id, _archiveId, _name, _parentId, _physicalFolderId, _isShared, _size);
        }
        
        if (_tenantId.HasValue)
        {
            entry.TenantId = _tenantId;
        }
        
        return entry;
    }
}

public class FileArchiveBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "TestArchive";
    private FileArchiveHierarchyType _hierarchyType = FileArchiveHierarchyType.Physical;
    private Guid _storageProviderId = Guid.NewGuid();
    private Guid _fileEditStorageProviderId = Guid.NewGuid();
    private string _rootFolderId = Guid.NewGuid().ToString();
    private string _physicalRootFolderId = Guid.NewGuid().ToString();
    private bool _isActive = true;
    private bool _isPersonalizedArchive = false;
    private Guid? _tenantId;

    public FileArchiveBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public FileArchiveBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public FileArchiveBuilder WithHierarchyType(FileArchiveHierarchyType hierarchyType)
    {
        _hierarchyType = hierarchyType;
        return this;
    }

    public FileArchiveBuilder WithStorageProviderId(Guid storageProviderId)
    {
        _storageProviderId = storageProviderId;
        return this;
    }

    public FileArchiveBuilder WithFileEditStorageProviderId(Guid fileEditStorageProviderId)
    {
        _fileEditStorageProviderId = fileEditStorageProviderId;
        return this;
    }

    public FileArchiveBuilder WithRootFolderId(string rootFolderId)
    {
        _rootFolderId = rootFolderId;
        return this;
    }

    public FileArchiveBuilder WithPhysicalRootFolderId(string physicalRootFolderId)
    {
        _physicalRootFolderId = physicalRootFolderId;
        return this;
    }

    public FileArchiveBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public FileArchiveBuilder WithIsPersonalizedArchive(bool isPersonalizedArchive)
    {
        _isPersonalizedArchive = isPersonalizedArchive;
        return this;
    }

    public FileArchiveBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public FileArchiveEntity Build()
    {
        return new FileArchiveEntity(_id)
        {
            Name = _name,
            FileArchiveHierarchyType = _hierarchyType,
            StorageProviderId = _storageProviderId,
            FileEditStorageProviderId = _fileEditStorageProviderId,
            RootFolderId = _rootFolderId,
            PhysicalRootFolderId = _physicalRootFolderId,
            IsActive = _isActive,
            IsPersonalizedArchive = _isPersonalizedArchive,
            TenantId = _tenantId
        };
    }
}

public class FileArchiveFavouriteBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _archiveId = Guid.NewGuid();
    private string _fileId = Guid.NewGuid().ToString();
    private string _folderId = Guid.NewGuid().ToString();
    private string _parentId = Guid.NewGuid().ToString();
    private string _userId = Guid.NewGuid().ToString();
    private Guid? _tenantId;

    public FileArchiveFavouriteBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public FileArchiveFavouriteBuilder WithArchiveId(Guid archiveId)
    {
        _archiveId = archiveId;
        return this;
    }

    public FileArchiveFavouriteBuilder WithFileId(string fileId)
    {
        _fileId = fileId;
        return this;
    }

    public FileArchiveFavouriteBuilder WithFolderId(string folderId)
    {
        _folderId = folderId;
        return this;
    }

    public FileArchiveFavouriteBuilder WithParentId(string parentId)
    {
        _parentId = parentId;
        return this;
    }

    public FileArchiveFavouriteBuilder WithUserId(string userId)
    {
        _userId = userId;
        return this;
    }

    public FileArchiveFavouriteBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public FileArchiveFavouriteEntity Build()
    {
        return new FileArchiveFavouriteEntity(_id, _archiveId, _fileId, _folderId, _parentId, _userId)
        {
            TenantId = _tenantId
        };
    }
}

public class FileArchivePermissionBuilder
{
    private Guid _archiveId = Guid.NewGuid();
    private string _folderId = Guid.NewGuid().ToString();
    private PermissionActorType _actorType = PermissionActorType.User;
    private string _actorId = Guid.NewGuid().ToString();
    private System.Collections.Generic.List<FileArchivePermissionTypeEntity> _permissionTypes = new();
    private Guid? _tenantId;

    public FileArchivePermissionBuilder WithArchiveId(Guid archiveId)
    {
        _archiveId = archiveId;
        return this;
    }

    public FileArchivePermissionBuilder WithFolderId(string folderId)
    {
        _folderId = folderId;
        return this;
    }

    public FileArchivePermissionBuilder WithActorType(PermissionActorType actorType)
    {
        _actorType = actorType;
        return this;
    }

    public FileArchivePermissionBuilder WithActorId(string actorId)
    {
        _actorId = actorId;
        return this;
    }

    public FileArchivePermissionBuilder WithPermissionType(FileManagerPermissionType permissionType)
    {
        _permissionTypes.Add(new FileArchivePermissionTypeEntity(Guid.NewGuid(), permissionType));
        return this;
    }

    public FileArchivePermissionBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public FileArchivePermissionEntity Build()
    {
        var permissionKey = new VPortal.FileManager.Module.ValueObjects.FileArchivePermissionKeyValueObject
        {
            ArchiveId = _archiveId,
            FolderId = _folderId,
            ActorType = _actorType,
            ActorId = _actorId
        };

        var entity = new FileArchivePermissionEntity(permissionKey)
        {
            TenantId = _tenantId,
            PermissionTypes = _permissionTypes
        };

        return entity;
    }
}

public class FileStatusBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _archiveId = Guid.NewGuid();
    private string _fileId = Guid.NewGuid().ToString();
    private string _folderId = Guid.NewGuid().ToString();
    private FileStatus _fileStatus = FileStatus.Active;
    private DateTime _statusDate = DateTime.UtcNow;
    private Guid? _tenantId;

    public FileStatusBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public FileStatusBuilder WithArchiveId(Guid archiveId)
    {
        _archiveId = archiveId;
        return this;
    }

    public FileStatusBuilder WithFileId(string fileId)
    {
        _fileId = fileId;
        return this;
    }

    public FileStatusBuilder WithFolderId(string folderId)
    {
        _folderId = folderId;
        return this;
    }

    public FileStatusBuilder WithFileStatus(FileStatus fileStatus)
    {
        _fileStatus = fileStatus;
        return this;
    }

    public FileStatusBuilder WithStatusDate(DateTime statusDate)
    {
        _statusDate = statusDate;
        return this;
    }

    public FileStatusBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public FileStatusEntity Build()
    {
        return new FileStatusEntity(_id, _archiveId, _fileId, _folderId, _fileStatus, _statusDate)
        {
            TenantId = _tenantId
        };
    }
}

public class FileExternalLinkBuilder
{
    private Guid _id = Guid.NewGuid();
    private FileShareStatus _permissionType = FileShareStatus.None;
    private Guid _archiveId = Guid.NewGuid();
    private string _fileId = Guid.NewGuid().ToString();
    private string _webUrl = "https://example.com/file";
    private string _externalFileId = Guid.NewGuid().ToString();
    private System.Collections.Generic.List<FileExternalLinkReviewerEntity> _reviewers = new();
    private Guid? _tenantId;

    public FileExternalLinkBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public FileExternalLinkBuilder WithPermissionType(FileShareStatus permissionType)
    {
        _permissionType = permissionType;
        return this;
    }

    public FileExternalLinkBuilder WithArchiveId(Guid archiveId)
    {
        _archiveId = archiveId;
        return this;
    }

    public FileExternalLinkBuilder WithFileId(string fileId)
    {
        _fileId = fileId;
        return this;
    }

    public FileExternalLinkBuilder WithWebUrl(string webUrl)
    {
        _webUrl = webUrl;
        return this;
    }

    public FileExternalLinkBuilder WithExternalFileId(string externalFileId)
    {
        _externalFileId = externalFileId;
        return this;
    }

    public FileExternalLinkBuilder WithReviewer(FileExternalLinkReviewerEntity reviewer)
    {
        _reviewers.Add(reviewer);
        return this;
    }

    public FileExternalLinkBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public FileExternalLinkEntity Build()
    {
        return new FileExternalLinkEntity(_id)
        {
            PermissionType = _permissionType,
            ArchiveId = _archiveId,
            FileId = _fileId,
            WebUrl = _webUrl,
            ExternalFileId = _externalFileId,
            Reviewers = _reviewers,
            TenantId = _tenantId
        };
    }
}
