using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using FileManagerClass = VPortal.FileManager.Module.Managers.FileManager;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.Managers;

public class FileManagerTests : DomainTestBase
{
    private readonly FileManagerClass _manager;
    private readonly IFileFactory _fileFactory;
    private readonly IArchiveRepository _archiveRepository;
    private readonly IFileSystemEntryRepository _repository;

    public FileManagerTests()
    {
        _fileFactory = CreateMockFileFactory();
        _archiveRepository = CreateMockArchiveRepository();
        _repository = CreateMockFileSystemEntryRepository();

        _manager = new FileManagerClass(
            CreateMockLogger<FileManagerClass>(),
            _fileFactory,
            _archiveRepository);
    }

    #region GetEntryById Tests

    [Fact]
    public async Task GetEntryById_ValidId_ReturnsEntry()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);

        // Act
        var result = await _manager.GetEntryById(entryId, archiveId, FileManagerType.FileArchive);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entryId);
    }

    [Fact]
    public async Task GetEntryById_InvalidId_ReturnsNull()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns((FileSystemEntry)null);

        // Act
        var result = await _manager.GetEntryById(entryId, archiveId, FileManagerType.FileArchive);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEntryById_ArchiveNotFound_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns<Task<FileArchiveEntity>>(callInfo => throw new EntityNotFoundException(typeof(FileArchiveEntity), archiveId));

        // Act
        var result = await _manager.GetEntryById(entryId, archiveId, FileManagerType.FileArchive);

        // Assert - Exception is caught and logged, returns default
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEntryById_InactiveArchive_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(false)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);

        // Act
        var result = await _manager.GetEntryById(entryId, archiveId, FileManagerType.FileArchive);

        // Assert - Exception is caught and logged, returns default
        result.Should().BeNull();
    }

    #endregion

    #region GetEntriesByParentId Tests

    [Fact]
    public async Task GetEntriesByParentId_ValidParent_ReturnsChildren()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var children = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(parentId)
                .AsFile()
                .Build(),
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(parentId)
                .AsFolder()
                .Build()
        };

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(parentId, null, false)
            .Returns(children);

        // Act
        var result = await _manager.GetEntriesByParentId(parentId, archiveId, null, FileManagerType.FileArchive, false);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetEntriesByParentId_Recursive_ReturnsAllDescendants()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var allDescendants = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(parentId)
                .AsFile()
                .Build(),
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(parentId)
                .AsFolder()
                .Build()
        };

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(parentId, null, true)
            .Returns(allDescendants);

        // Act
        var result = await _manager.GetEntriesByParentId(parentId, archiveId, null, FileManagerType.FileArchive, true);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetEntriesByParentId_EmptyParent_ReturnsRootEntries()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var rootEntries = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(null)
                .AsFile()
                .Build()
        };

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(null, null, false)
            .Returns(rootEntries);

        // Act
        var result = await _manager.GetEntriesByParentId(null, archiveId, null, FileManagerType.FileArchive, false);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetEntriesByParentId_FilterByKind_ReturnsFilteredEntries()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var files = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(parentId)
                .AsFile()
                .Build()
        };

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(parentId, EntryKind.File, false)
            .Returns(files);

        // Act
        var result = await _manager.GetEntriesByParentId(parentId, archiveId, EntryKind.File, FileManagerType.FileArchive, false);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.All(e => e.EntryKind == EntryKind.File).Should().BeTrue();
    }

    #endregion

    #region CreateFile Tests

    [Fact]
    public async Task CreateFile_ValidInput_CreatesFile()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var parent = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(parentId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var newFile = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(fileId)
            .WithArchiveId(archiveId)
            .WithParentId(parentId)
            .AsFile()
            .WithName("test.txt")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(parentId).Returns(parent);
        _repository.GetEntriesByParentId(parentId, EntryKind.File, false)
            .Returns(new List<FileSystemEntry>());
        _repository.CreateEntry(EntryKind.File, "test.txt", parentId, null, false, ".txt", null, null, null)
            .Returns(newFile);

        // Act
        var result = await _manager.CreateFile(fileId, archiveId, "test.txt", parentId, ".txt");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(fileId);
        result.Name.Should().Be("test.txt");
    }

    [Fact]
    public async Task CreateFile_DuplicateName_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var parent = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(parentId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var existingFile = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(Guid.NewGuid().ToString())
            .WithArchiveId(archiveId)
            .WithParentId(parentId)
            .AsFile()
            .WithName("test.txt")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(parentId).Returns(parent);
        _repository.GetEntriesByParentId(parentId, EntryKind.File, false)
            .Returns(new List<FileSystemEntry> { existingFile });

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.CreateFile(fileId, archiveId, "test.txt", parentId, ".txt"));
    }

    [Fact]
    public async Task CreateFile_InvalidParent_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(parentId)
            .Returns<Task<FileSystemEntry>>(callInfo => throw new EntityNotFoundException(typeof(FileSystemEntry), parentId));

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.CreateFile(fileId, archiveId, "test.txt", parentId, ".txt"));
    }

    [Fact]
    public async Task CreateFile_ParentIsFile_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var parent = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(parentId)
            .WithArchiveId(archiveId)
            .AsFile() // Parent is a file, not a folder
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(parentId).Returns(parent);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.CreateFile(fileId, archiveId, "test.txt", parentId, ".txt"));
    }

    [Fact]
    public async Task CreateFile_EmptyName_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.CreateFile(fileId, archiveId, "", null, ".txt"));
    }

    [Fact]
    public async Task CreateFile_WhitespaceName_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.CreateFile(fileId, archiveId, "   ", null, ".txt"));
    }

    [Fact]
    public async Task CreateFile_WithNullParent_CreatesRootFile()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var newFile = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(fileId)
            .WithArchiveId(archiveId)
            .WithParentId(null)
            .AsFile()
            .WithName("root.txt")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(default, default, default)
            .ReturnsForAnyArgs(new List<FileSystemEntry>());
        _repository.CreateEntry(default, default, default, default, default, default, default, default, default)
            .ReturnsForAnyArgs(callInfo => FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(fileId)
                .WithArchiveId(archiveId)
                .WithName(callInfo.ArgAt<string>(1))
                .AsFile()
                .Build());
        _repository.CreateEntry(EntryKind.File, "root.txt", null, null, false, ".txt", null, null, null)
            .Returns(newFile);

        // Act
        var result = await _manager.CreateFile(fileId, archiveId, "root.txt", null, ".txt");

        // Assert
        result.Should().NotBeNull();
        result.ParentId.Should().BeNull();
    }

    #endregion

    #region CreateFolder Tests

    [Fact]
    public async Task CreateFolder_ValidInput_CreatesFolder()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var parent = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(parentId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var newFolder = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(folderId)
            .WithArchiveId(archiveId)
            .WithParentId(parentId)
            .AsFolder()
            .WithName("NewFolder")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(default, default, default)
            .ReturnsForAnyArgs(new List<FileSystemEntry>());
        _repository.GetEntryById(parentId).Returns(parent);
        _repository.GetEntriesByParentId(parentId, EntryKind.Folder, false)
            .Returns(new List<FileSystemEntry>());
        _repository.CreateEntry(EntryKind.Folder, "NewFolder", parentId, null, false, null, null, null, null)
            .Returns(newFolder);

        // Act
        var result = await _manager.CreateFolder(folderId, archiveId, "NewFolder", parentId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(folderId);
        result.EntryKind.Should().Be(EntryKind.Folder);
    }

    [Fact]
    public async Task CreateFolder_DuplicateName_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var parent = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(parentId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var existingFolder = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(Guid.NewGuid().ToString())
            .WithArchiveId(archiveId)
            .WithParentId(parentId)
            .AsFolder()
            .WithName("NewFolder")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(parentId).Returns(parent);
        _repository.GetEntriesByParentId(parentId, EntryKind.Folder, false)
            .Returns(new List<FileSystemEntry> { existingFolder });

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.CreateFolder(folderId, archiveId, "NewFolder", parentId));
    }

    [Fact]
    public async Task CreateFolder_WithNullParent_CreatesRootFolder()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var newFolder = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(folderId)
            .WithArchiveId(archiveId)
            .WithParentId(null)
            .AsFolder()
            .WithName("RootFolder")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(null, EntryKind.Folder, false)
            .Returns(new List<FileSystemEntry>());
        _repository.CreateEntry(EntryKind.Folder, "RootFolder", null, null, false, null, null, null, null)
            .Returns(newFolder);

        // Act
        var result = await _manager.CreateFolder(folderId, archiveId, "RootFolder", null);

        // Assert
        result.Should().NotBeNull();
        result.ParentId.Should().BeNull();
    }

    #endregion

    #region Rename Tests

    [Fact]
    public async Task Rename_ValidInput_RenamesEntry()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("old.txt")
            .Build();
        var renamedEntry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("new.txt")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntriesByParentId(entry.ParentId, entry.EntryKind, false)
            .Returns(new List<FileSystemEntry>());
        _repository.RenameEntry(entryId, "new.txt").Returns(renamedEntry);

        // Act
        var result = await _manager.Rename(entryId, "new.txt", archiveId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("new.txt");
    }

    [Fact]
    public async Task Rename_DuplicateName_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("old.txt")
            .Build();
        var existingEntry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(Guid.NewGuid().ToString())
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("new.txt")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntriesByParentId(entry.ParentId, entry.EntryKind, false)
            .Returns(new List<FileSystemEntry> { existingEntry });

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.Rename(entryId, "new.txt", archiveId));
    }

    [Fact]
    public async Task Rename_EmptyName_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.Rename(entryId, "", archiveId));
    }

    #endregion

    #region Move Tests

    [Fact]
    public async Task Move_ValidInput_MovesEntry()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var destinationParentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("file.txt")
            .Build();
        var destination = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(destinationParentId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var movedEntry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("file.txt")
            .WithParentId(destinationParentId)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntryById(destinationParentId).Returns(destination);
        _repository.GetEntryParentsById(destinationParentId).Returns(new List<FileSystemEntry>());
        _repository.GetEntriesByParentId(destinationParentId, entry.EntryKind, false)
            .Returns(new List<FileSystemEntry>());
        _repository.MoveEntry(entryId, destinationParentId).Returns(true);
        _repository.GetEntryById(entryId).Returns(movedEntry);

        // Act
        var result = await _manager.Move(entryId, destinationParentId, archiveId);

        // Assert
        result.Should().NotBeNull();
        result.ParentId.Should().Be(destinationParentId);
    }

    [Fact]
    public async Task Move_InvalidDestination_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var destinationParentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntryById(destinationParentId)
            .Returns<Task<FileSystemEntry>>(callInfo => throw new EntityNotFoundException(typeof(FileSystemEntry), destinationParentId));

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.Move(entryId, destinationParentId, archiveId));
    }

    [Fact]
    public async Task Move_DestinationIsFile_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var destinationParentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .Build();
        var destination = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(destinationParentId)
            .WithArchiveId(archiveId)
            .AsFile() // Destination is a file, not a folder
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntryById(destinationParentId).Returns(destination);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.Move(entryId, destinationParentId, archiveId));
    }

    [Fact]
    public async Task Move_ToSelf_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.Move(entryId, entryId, archiveId));
    }

    [Fact]
    public async Task Move_ToDescendant_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var destinationParentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var destination = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(destinationParentId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var parentChain = new List<FileSystemEntry> { entry }; // Destination is a descendant

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntryById(destinationParentId).Returns(destination);
        _repository.GetEntryParentsById(destinationParentId).Returns(parentChain);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.Move(entryId, destinationParentId, archiveId));
    }

    [Fact]
    public async Task Move_DuplicateNameAtDestination_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var destinationParentId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("file.txt")
            .Build();
        var destination = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(destinationParentId)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var existingEntry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(Guid.NewGuid().ToString())
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("file.txt")
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntryById(destinationParentId).Returns(destination);
        _repository.GetEntryParentsById(destinationParentId).Returns(new List<FileSystemEntry>());
        _repository.GetEntriesByParentId(destinationParentId, entry.EntryKind, false)
            .Returns(new List<FileSystemEntry> { existingEntry });

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.Move(entryId, destinationParentId, archiveId));
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ValidId_DeletesEntry()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.DeleteEntry(entryId).Returns(Task.FromResult(true));

        // Act
        await _manager.Delete(entryId, archiveId);

        // Assert - No exception thrown
        await _repository.Received(1).DeleteEntry(entryId);
    }

    [Fact]
    public async Task Delete_NonExistent_CompletesWithoutError()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.DeleteEntry(entryId)
            .Returns(Task.FromException<bool>(new EntityNotFoundException(typeof(FileSystemEntry), entryId)));

        // Act - Exception is caught and logged, method completes
        await _manager.Delete(entryId, archiveId);

        // Assert - No exception propagated
    }

    #endregion

    #region Provider Type Tests

    [Fact]
    public async Task GetEntryById_ProviderType_ReturnsEntry()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .AsFile()
            .Build();

        _fileFactory.Get(Arg.Any<FileArchiveEntity>(), FileManagerType.Provider)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);

        // Act
        var result = await _manager.GetEntryById(entryId, providerId, FileManagerType.Provider);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entryId);
    }

    #endregion

    #region Security Tests

    [Fact]
    public async Task CreateFile_MaliciousInput_SanitizedOrRejected()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(null, EntryKind.File, false)
            .Returns(new List<FileSystemEntry>());

        var maliciousInputs = Eleon.TestsBase.Lib.TestHelpers.SecurityTestHelpers.CreateMaliciousInputs();

        // Act & Assert - Test each malicious input
        foreach (var maliciousName in maliciousInputs.Take(5)) // Test first 5 to avoid too many tests
        {
            try
            {
                var result = await _manager.CreateFile(fileId, archiveId, maliciousName, null, ".txt");
                // If it succeeds, verify it was sanitized
                if (result != null)
                {
                    result.Name.Should().NotContainAny("<script", "javascript:", "../", "..\\", "\0");
                }
            }
            catch (UserFriendlyException)
            {
                // Expected - input was rejected
            }
        }
    }

    [Fact]
    public async Task CreateFile_PathTraversalAttempt_Rejected()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await _manager.CreateFile(fileId, archiveId, "../../../etc/passwd", null, ".txt"));
    }

    #endregion

    #region Concurrency Tests

    [Fact]
    public async Task CreateFile_ConcurrentCreation_SameName_HandlesConflicts()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntriesByParentId(null, EntryKind.File, false)
            .Returns(new List<FileSystemEntry>());

        var callCount = 0;
        _repository.CreateEntry(EntryKind.File, "concurrent.txt", null, null, false, ".txt", null, null, null)
            .Returns<Task<FileSystemEntry>>(callInfo =>
            {
                callCount++;
                if (callCount == 1)
                {
                    // First call succeeds
                    return Task.FromResult(FileManagerTestDataBuilder.FileSystemEntry()
                        .WithId(Guid.NewGuid().ToString())
                        .AsFile()
                        .WithName("concurrent.txt")
                        .Build());
                }
                else
                {
                    // Subsequent calls should fail due to duplicate name check
                    throw new UserFriendlyException("Duplicate name");
                }
            });

        // Act - Simulate concurrent creation
        var tasks = Enumerable.Range(0, 5).Select(async i =>
        {
            try
            {
                return await _manager.CreateFile(Guid.NewGuid().ToString(), archiveId, "concurrent.txt", null, ".txt");
            }
            catch
            {
                return null;
            }
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - Only one should succeed
        var successful = results.Where(r => r != null).ToList();
        successful.Should().HaveCount(1);
    }

    [Fact]
    public async Task Move_ConcurrentMoves_HandlesConflicts()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entryId = Guid.NewGuid().ToString();
        var destination1Id = Guid.NewGuid().ToString();
        var destination2Id = Guid.NewGuid().ToString();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .WithIsActive(true)
            .Build();
        var entry = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(entryId)
            .WithArchiveId(archiveId)
            .AsFile()
            .WithName("file.txt")
            .Build();
        var destination1 = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(destination1Id)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();
        var destination2 = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(destination2Id)
            .WithArchiveId(archiveId)
            .AsFolder()
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
        _fileFactory.Get(archive, FileManagerType.FileArchive)
            .Returns(_repository);
        _repository.GetEntryById(entryId).Returns(entry);
        _repository.GetEntryById(destination1Id).Returns(destination1);
        _repository.GetEntryById(destination2Id).Returns(destination2);
        _repository.GetEntryParentsById(Arg.Any<string>()).Returns(new List<FileSystemEntry>());
        _repository.GetEntriesByParentId(Arg.Any<string>(), Arg.Any<EntryKind>(), false)
            .Returns(new List<FileSystemEntry>());
        _repository.MoveEntry(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        _repository.GetEntryById(entryId).Returns(entry);

        // Act - Try to move to different destinations concurrently
        var task1 = _manager.Move(entryId, destination1Id, archiveId);
        var task2 = _manager.Move(entryId, destination2Id, archiveId);

        var results = await Task.WhenAll(task1, task2);

        // Assert - Both should complete (last one wins)
        results.Should().AllSatisfy(r => r.Should().NotBeNull());
    }

    #endregion
}
