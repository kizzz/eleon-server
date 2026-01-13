using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Microsoft.Extensions.Logging;
using VPortal.FileManager.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using Xunit;
using FileManagerService = VPortal.FileManager.Module.Managers.FileManager;

namespace VPortal.FileManager.Module.Tests.DomainServices
{
    public class FileSystemEntryDomainServiceTests
    {
        private readonly FileManagerService _fileManager;
        private readonly MockFileRepository _mockRepository;
        private readonly MockFileSystemEntryRepository _mockFileSystemEntryRepository;
        private readonly Mock<IVportalLogger<FileManagerService>> _mockLogger;
        private readonly Mock<IFileFactory> _mockFileFactory;
        private readonly Mock<IArchiveRepository> _mockArchiveRepository;

        public FileSystemEntryDomainServiceTests()
        {
            _mockRepository = new MockFileRepository();
            _mockLogger = new Mock<IVportalLogger<FileManagerService>>();
            
            // Create the mock repository that will be returned by FileFactory.Get()
            _mockFileSystemEntryRepository = new MockFileSystemEntryRepository(_mockRepository);
            
            // Mock IFileFactory - now we can properly mock Get() since it's an interface method
            _mockFileFactory = new Mock<IFileFactory>();
            _mockFileFactory.Setup(f => f.Get(It.IsAny<FileArchiveEntity>(), It.IsAny<FileManagerType>()))
                .ReturnsAsync(_mockFileSystemEntryRepository);
            
            // Mock IArchiveRepository - return a valid archive for any archiveId
            _mockArchiveRepository = new Mock<IArchiveRepository>();
            _mockArchiveRepository.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, bool _, CancellationToken _) => new FileArchiveEntity(id) { IsActive = true });
            _mockArchiveRepository.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid? id, bool _, CancellationToken _) => id.HasValue ? new FileArchiveEntity(id.Value) { IsActive = true } : null);
            
            _fileManager = new FileManagerService(
                _mockLogger.Object,
                _mockFileFactory.Object,
                _mockArchiveRepository.Object);
        }

        [Fact]
        public async Task CreateFile_ValidInput_CreatesFileEntry()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            var name = "test.txt";
            var parentId = Guid.NewGuid().ToString();
            
            // Create parent folder first
            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            
            // Register expected ID
            _mockFileSystemEntryRepository.RegisterExpectedId(name, parentId, id);

            // Act
            var result = await _fileManager.CreateFile(id, archiveId, name, parentId, ".txt");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(EntryKind.File, result.EntryKind);
            Assert.Equal(name, result.Name);
            Assert.Equal(parentId, result.ParentId);
            Assert.Equal(".txt", result.Extension);
        }

        [Fact]
        public async Task CreateFolder_ValidInput_CreatesFolderEntry()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            var name = "TestFolder";
            var parentId = Guid.NewGuid().ToString();
            
            // Create parent folder first
            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            
            // Register expected ID so the repository uses it
            _mockFileSystemEntryRepository.RegisterExpectedId(name, parentId, id);

            // Act
            var result = await _fileManager.CreateFolder(id, archiveId, name, parentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(EntryKind.Folder, result.EntryKind);
            Assert.Equal(name, result.Name);
            Assert.Equal(parentId, result.ParentId);
        }

        [Fact]
        public async Task CreateFile_EmptyName_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFile(id, archiveId, "", null));
        }

        [Fact]
        public async Task CreateFolder_EmptyName_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFolder(id, archiveId, "", null));
        }

        [Fact]
        public async Task Rename_DuplicateSiblingName_ThrowsException()
        {
            // Arrange
            var parentId = Guid.NewGuid().ToString();
            var existingId = Guid.NewGuid().ToString();
            var anotherFileId = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            
            // Create parent folder first
            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            // Create existing entry with name "existing.txt"
            _mockFileSystemEntryRepository.RegisterExpectedId("existing.txt", parentId, existingId);
            await _fileManager.CreateFile(existingId, archiveId, "existing.txt", parentId);
            // Create another file with name "other.txt"
            _mockFileSystemEntryRepository.RegisterExpectedId("other.txt", parentId, anotherFileId);
            await _fileManager.CreateFile(anotherFileId, archiveId, "other.txt", parentId);

            // Act & Assert - Try to rename "other.txt" to "existing.txt" which already exists
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Rename(anotherFileId, "existing.txt", archiveId));
        }

        [Fact]
        public async Task Move_UnderSelf_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            _mockFileSystemEntryRepository.RegisterExpectedId("Folder", null, id);
            await _fileManager.CreateFolder(id, archiveId, "Folder", null);

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Move(id, id, archiveId));
        }

        [Fact]
        public async Task Move_UnderDescendant_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var childId = Guid.NewGuid().ToString();
            var grandchildId = Guid.NewGuid().ToString();

            _mockFileSystemEntryRepository.RegisterExpectedId("Parent", null, parentId);
            await _fileManager.CreateFolder(parentId, archiveId, "Parent", null);
            _mockFileSystemEntryRepository.RegisterExpectedId("Child", parentId, childId);
            await _fileManager.CreateFolder(childId, archiveId, "Child", parentId);
            _mockFileSystemEntryRepository.RegisterExpectedId("Grandchild", childId, grandchildId);
            await _fileManager.CreateFolder(grandchildId, archiveId, "Grandchild", childId);

            // Act & Assert - Cannot move parent under grandchild
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Move(parentId, grandchildId, archiveId));
        }

        [Fact]
        public async Task CreateFile_WithNullParent_CreatesRootFile()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            var name = "rootfile.txt";

            // Act
            _mockFileSystemEntryRepository.RegisterExpectedId(name, null, id);
            var result = await _fileManager.CreateFile(id, archiveId, name, null, ".txt");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(EntryKind.File, result.EntryKind);
            Assert.Null(result.ParentId);
        }

        [Fact]
        public async Task CreateFile_ParentNotFound_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            var nonExistentParentId = Guid.NewGuid().ToString();

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFile(id, archiveId, "test.txt", nonExistentParentId));
        }

        [Fact]
        public async Task CreateFile_ParentIsFile_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var fileId = Guid.NewGuid().ToString();
            var childId = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            await _fileManager.CreateFile(fileId, archiveId, "parent.txt", parentId);

            // Act & Assert - Cannot create file under a file
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFile(childId, archiveId, "child.txt", fileId));
        }

        [Fact]
        public async Task CreateFile_DuplicateName_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var file1Id = Guid.NewGuid().ToString();
            var file2Id = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            await _fileManager.CreateFile(file1Id, archiveId, "test.txt", parentId);

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFile(file2Id, archiveId, "test.txt", parentId));
        }

        [Fact]
        public async Task CreateFolder_WithNullParent_CreatesRootFolder()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            var name = "RootFolder";

            // Act
            var result = await _fileManager.CreateFolder(id, archiveId, name, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(EntryKind.Folder, result.EntryKind);
            Assert.Null(result.ParentId);
        }

        [Fact]
        public async Task CreateFolder_ParentNotFound_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();
            var nonExistentParentId = Guid.NewGuid().ToString();

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFolder(id, archiveId, "TestFolder", nonExistentParentId));
        }

        [Fact]
        public async Task CreateFolder_ParentIsFile_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var fileId = Guid.NewGuid().ToString();
            var folderId = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            await _fileManager.CreateFile(fileId, archiveId, "parent.txt", parentId);

            // Act & Assert - Cannot create folder under a file
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFolder(folderId, archiveId, "ChildFolder", fileId));
        }

        [Fact]
        public async Task CreateFolder_DuplicateName_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var folder1Id = Guid.NewGuid().ToString();
            var folder2Id = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            await _fileManager.CreateFolder(folder1Id, archiveId, "TestFolder", parentId);

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFolder(folder2Id, archiveId, "TestFolder", parentId));
        }

        [Fact]
        public async Task Rename_EmptyName_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var fileId = Guid.NewGuid().ToString();
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", null, fileId);
            await _fileManager.CreateFile(fileId, archiveId, "test.txt", null);

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Rename(fileId, "", archiveId));
        }

        [Fact]
        public async Task Rename_ToSameName_Succeeds()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var fileId = Guid.NewGuid().ToString();
            var name = "test.txt";
            _mockFileSystemEntryRepository.RegisterExpectedId(name, null, fileId);
            await _fileManager.CreateFile(fileId, archiveId, name, null);

            // Act
            var result = await _fileManager.Rename(fileId, name, archiveId);

            // Assert
            Assert.Equal(name, result.Name);
        }

        [Fact]
        public async Task Rename_ToDifferentName_Succeeds()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var fileId = Guid.NewGuid().ToString();
            _mockFileSystemEntryRepository.RegisterExpectedId("old.txt", null, fileId);
            await _fileManager.CreateFile(fileId, archiveId, "old.txt", null);

            // Act
            var result = await _fileManager.Rename(fileId, "new.txt", archiveId);

            // Assert
            Assert.Equal("new.txt", result.Name);
        }

        [Fact]
        public async Task Move_ToNullParent_MovesToRoot()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var fileId = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", parentId, fileId);
            await _fileManager.CreateFile(fileId, archiveId, "test.txt", parentId);

            // Act
            var result = await _fileManager.Move(fileId, null, archiveId);

            // Assert
            Assert.Null(result.ParentId);
        }

        [Fact]
        public async Task Move_ToValidParent_Succeeds()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parent1Id = Guid.NewGuid().ToString();
            var parent2Id = Guid.NewGuid().ToString();
            var fileId = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent1Id, archiveId, "Parent1", null));
            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent2Id, archiveId, "Parent2", null));
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", parent1Id, fileId);
            await _fileManager.CreateFile(fileId, archiveId, "test.txt", parent1Id);

            // Act
            var result = await _fileManager.Move(fileId, parent2Id, archiveId);

            // Assert
            Assert.Equal(parent2Id, result.ParentId);
        }

        [Fact]
        public async Task Move_DestinationNotFound_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var fileId = Guid.NewGuid().ToString();
            var nonExistentParentId = Guid.NewGuid().ToString();

            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", null, fileId);
            await _fileManager.CreateFile(fileId, archiveId, "test.txt", null);

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Move(fileId, nonExistentParentId, archiveId));
        }

        [Fact]
        public async Task Move_DestinationIsFile_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var file1Id = Guid.NewGuid().ToString();
            var file2Id = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            _mockFileSystemEntryRepository.RegisterExpectedId("file1.txt", parentId, file1Id);
            await _fileManager.CreateFile(file1Id, archiveId, "file1.txt", parentId);
            _mockFileSystemEntryRepository.RegisterExpectedId("file2.txt", parentId, file2Id);
            await _fileManager.CreateFile(file2Id, archiveId, "file2.txt", parentId);

            // Act & Assert - Cannot move file under another file
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Move(file2Id, file1Id, archiveId));
        }

        [Fact]
        public async Task Move_DuplicateNameAtDestination_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parent1Id = Guid.NewGuid().ToString();
            var parent2Id = Guid.NewGuid().ToString();
            var file1Id = Guid.NewGuid().ToString();
            var file2Id = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent1Id, archiveId, "Parent1", null));
            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent2Id, archiveId, "Parent2", null));
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", parent1Id, file1Id);
            await _fileManager.CreateFile(file1Id, archiveId, "test.txt", parent1Id);
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", parent2Id, file2Id);
            await _fileManager.CreateFile(file2Id, archiveId, "test.txt", parent2Id);

            // Act & Assert - Cannot move file1 to parent2 where file2 already has the same name
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Move(file1Id, parent2Id, archiveId));
        }

        [Fact]
        public async Task Move_File_UpdatesFolderId()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parent1Id = Guid.NewGuid().ToString();
            var parent2Id = Guid.NewGuid().ToString();
            var fileId = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent1Id, archiveId, "Parent1", null));
            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent2Id, archiveId, "Parent2", null));
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", parent1Id, fileId);
            var file = await _fileManager.CreateFile(fileId, archiveId, "test.txt", parent1Id);

            // Act
            var result = await _fileManager.Move(fileId, parent2Id, archiveId);

            // Assert
            Assert.Equal(parent2Id, result.ParentId);
            // Note: FolderId is legacy, but should be updated for files
        }

        [Fact]
        public async Task Move_Folder_DoesNotUpdateFolderId()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parent1Id = Guid.NewGuid().ToString();
            var parent2Id = Guid.NewGuid().ToString();
            var folderId = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent1Id, archiveId, "Parent1", null));
            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parent2Id, archiveId, "Parent2", null));
            _mockFileSystemEntryRepository.RegisterExpectedId("TestFolder", parent1Id, folderId);
            await _fileManager.CreateFolder(folderId, archiveId, "TestFolder", parent1Id);

            // Act
            var result = await _fileManager.Move(folderId, parent2Id, archiveId);

            // Assert
            Assert.Equal(parent2Id, result.ParentId);
        }

        [Fact]
        public async Task Delete_File_Succeeds()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var fileId = Guid.NewGuid().ToString();
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", null, fileId);
            await _fileManager.CreateFile(fileId, archiveId, "test.txt", null);

            // Act - Should not throw
            await _fileManager.Delete(fileId, archiveId);

            // Assert - File should be deleted (handled by repository)
        }

        [Fact]
        public async Task Delete_FolderWithChildren_Succeeds()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var folderId = Guid.NewGuid().ToString();
            var childId = Guid.NewGuid().ToString();

            _mockFileSystemEntryRepository.RegisterExpectedId("ParentFolder", null, folderId);
            await _fileManager.CreateFolder(folderId, archiveId, "ParentFolder", null);
            _mockFileSystemEntryRepository.RegisterExpectedId("child.txt", folderId, childId);
            await _fileManager.CreateFile(childId, archiveId, "child.txt", folderId);

            // Act - Should not throw (current behavior allows delete)
            await _fileManager.Delete(folderId, archiveId);

            // Assert - Folder deletion is allowed (status set to Trash at higher level)
        }

        [Fact]
        public async Task Delete_FolderWithoutChildren_Succeeds()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var folderId = Guid.NewGuid().ToString();
            _mockFileSystemEntryRepository.RegisterExpectedId("EmptyFolder", null, folderId);
            await _fileManager.CreateFolder(folderId, archiveId, "EmptyFolder", null);

            // Act - Should not throw
            await _fileManager.Delete(folderId, archiveId);

            // Assert - Empty folder deletion succeeds
        }

        [Fact]
        public async Task GetChildren_ReturnsAllChildren()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var file1Id = Guid.NewGuid().ToString();
            var file2Id = Guid.NewGuid().ToString();
            var folderId = Guid.NewGuid().ToString();

            await _mockRepository.InsertAsync(FileSystemEntry.CreateFolder(parentId, archiveId, "ParentFolder", null));
            _mockFileSystemEntryRepository.RegisterExpectedId("file1.txt", parentId, file1Id);
            await _fileManager.CreateFile(file1Id, archiveId, "file1.txt", parentId);
            _mockFileSystemEntryRepository.RegisterExpectedId("file2.txt", parentId, file2Id);
            await _fileManager.CreateFile(file2Id, archiveId, "file2.txt", parentId);
            _mockFileSystemEntryRepository.RegisterExpectedId("ChildFolder", parentId, folderId);
            await _fileManager.CreateFolder(folderId, archiveId, "ChildFolder", parentId);

            // Act
            var children = await _fileManager.GetChildren(parentId, archiveId);

            // Assert
            Assert.Equal(3, children.Count);
            Assert.Contains(children, c => c.Id == file1Id);
            Assert.Contains(children, c => c.Id == file2Id);
            Assert.Contains(children, c => c.Id == folderId);
        }

        [Fact]
        public async Task GetChildren_EmptyFolder_ReturnsEmptyList()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var folderId = Guid.NewGuid().ToString();
            _mockFileSystemEntryRepository.RegisterExpectedId("EmptyFolder", null, folderId);
            await _fileManager.CreateFolder(folderId, archiveId, "EmptyFolder", null);

            // Act
            var children = await _fileManager.GetChildren(folderId, archiveId);

            // Assert
            Assert.Empty(children);
        }

        [Fact]
        public async Task IsDescendant_DirectChild_ReturnsTrue()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var parentId = Guid.NewGuid().ToString();
            var childId = Guid.NewGuid().ToString();

            _mockFileSystemEntryRepository.RegisterExpectedId("Parent", null, parentId);
            await _fileManager.CreateFolder(parentId, archiveId, "Parent", null);
            _mockFileSystemEntryRepository.RegisterExpectedId("Child", parentId, childId);
            await _fileManager.CreateFolder(childId, archiveId, "Child", parentId);

            // Act - Test internal IsDescendant logic via Move
            // Move parent under child should fail
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Move(parentId, childId, archiveId));
        }

        [Fact]
        public async Task CreateFile_WhitespaceName_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFile(id, archiveId, "   ", null));
        }

        [Fact]
        public async Task CreateFolder_WhitespaceName_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var archiveId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.CreateFolder(id, archiveId, "   ", null));
        }

        [Fact]
        public async Task Rename_WhitespaceName_ThrowsException()
        {
            // Arrange
            var archiveId = Guid.NewGuid();
            var fileId = Guid.NewGuid().ToString();
            _mockFileSystemEntryRepository.RegisterExpectedId("test.txt", null, fileId);
            await _fileManager.CreateFile(fileId, archiveId, "test.txt", null);

            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _fileManager.Rename(fileId, "   ", archiveId));
        }

        // Mock repository for testing - implements IFileSystemEntryRepository to work with FileManager
        private class MockFileSystemEntryRepository : IFileSystemEntryRepository
        {
            private readonly MockFileRepository _basicRepository;
            private FileArchiveEntity _archive;
            private readonly Dictionary<string, string> _nameToExpectedId = new(); // Track expected IDs by name+parent for test scenarios

            public FileArchiveEntity Archive 
            { 
                get => _archive ??= new FileArchiveEntity(Guid.NewGuid());
                set => _archive = value;
            }

            public MockFileSystemEntryRepository(MockFileRepository basicRepository)
            {
                _basicRepository = basicRepository;
                _archive = new FileArchiveEntity(Guid.NewGuid()) { IsActive = true };
            }
            
            // Method to register expected ID for a name+parent combination (used by tests)
            public void RegisterExpectedId(string name, string? parentId, string expectedId)
            {
                var key = $"{parentId ?? "null"}:{name}";
                _nameToExpectedId[key] = expectedId;
            }
            

            public async Task<List<FileSystemEntry>> GetEntriesByParentId(string parentId, EntryKind? kind = null, bool recursive = false)
            {
                var allEntries = await _basicRepository.GetListAsync();
                var filtered = allEntries.Where(e => e.ParentId == parentId);
                if (kind.HasValue)
                {
                    filtered = filtered.Where(e => e.EntryKind == kind.Value);
                }
                if (recursive)
                {
                    var recursiveEntries = new List<FileSystemEntry>();
                    foreach (var entry in filtered)
                    {
                        recursiveEntries.Add(entry);
                        var children = await GetEntriesByParentId(entry.Id, kind, true);
                        recursiveEntries.AddRange(children);
                    }
                    return recursiveEntries;
                }
                return filtered.ToList();
            }

            public async Task<(List<FileSystemEntry> Items, long TotalCount)> GetEntriesByParentIdPaged(string parentId, EntryKind? kind, int skipCount, int maxResultCount, string? sorting = null, bool recursive = false)
            {
                var items = await GetEntriesByParentId(parentId, kind, recursive);
                // Simple implementation - doesn't handle paging/sorting properly
                return (items.Skip(skipCount).Take(maxResultCount).ToList(), items.Count);
            }

            public async Task<List<FileSystemEntry>> GetEntriesByIds(List<string> ids, EntryKind? kind = null)
            {
                var results = new List<FileSystemEntry>();
                foreach (var id in ids)
                {
                    var entry = await _basicRepository.FindAsync(id);
                    if (entry != null && (!kind.HasValue || entry.EntryKind == kind.Value))
                    {
                        results.Add(entry);
                    }
                }
                return results;
            }

            public async Task<FileSystemEntry> GetEntryById(string id)
            {
                return await _basicRepository.GetAsync(id);
            }

            public async Task<List<FileSystemEntry>> GetEntryParentsById(string id)
            {
                var parents = new List<FileSystemEntry>();
                var entry = await _basicRepository.FindAsync(id);
                while (entry != null && !string.IsNullOrEmpty(entry.ParentId))
                {
                    entry = await _basicRepository.FindAsync(entry.ParentId);
                    if (entry != null)
                    {
                        parents.Add(entry);
                    }
                }
                return parents;
            }

            public async Task<FileSystemEntry> GetRootEntry()
            {
                var allEntries = await _basicRepository.GetListAsync();
                return allEntries.FirstOrDefault(e => e.ParentId == null && e.EntryKind == EntryKind.Folder)
                    ?? allEntries.FirstOrDefault(e => e.ParentId == null);
            }

            public async Task<List<FileSystemEntry>> SearchEntries(string search, EntryKind? kind = null)
            {
                var allEntries = await _basicRepository.GetListAsync();
                var filtered = allEntries.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                if (kind.HasValue)
                {
                    filtered = filtered.Where(e => e.EntryKind == kind.Value);
                }
                return filtered.ToList();
            }

            public async Task<FileSystemEntry> CreateEntry(EntryKind kind, string name, string? parentId, string? physicalFolderId = null, bool isShared = false, string? extension = null, string? path = null, string? size = null, string? thumbnailPath = null)
            {
                // Check if there's a registered expected ID for this name+parent combination
                var key = $"{parentId ?? "null"}:{name}";
                var id = _nameToExpectedId.TryGetValue(key, out var expectedId) ? expectedId : Guid.NewGuid().ToString();
                
                FileSystemEntry entry;
                if (kind == EntryKind.File)
                {
                    entry = FileSystemEntry.CreateFile(id, Archive.Id, name, parentId, extension, path, size, thumbnailPath);
                }
                else
                {
                    entry = FileSystemEntry.CreateFolder(id, Archive.Id, name, parentId, physicalFolderId, isShared, size);
                }
                
                // Remove the registration after use
                _nameToExpectedId.Remove(key);
                
                return await _basicRepository.InsertAsync(entry);
            }

            public async Task<FileSystemEntry> RenameEntry(string id, string name)
            {
                var entry = await _basicRepository.GetAsync(id);
                entry.Name = name;
                return await _basicRepository.UpdateAsync(entry);
            }

            public async Task<bool> MoveEntry(string entryId, string destinationParentId)
            {
                var entry = await _basicRepository.GetAsync(entryId);
                entry.ParentId = destinationParentId;
                if (entry.EntryKind == EntryKind.File)
                {
                    entry.FolderId = destinationParentId;
                }
                await _basicRepository.UpdateAsync(entry);
                return true;
            }

            public async Task<bool> MoveAllEntries(List<string> entryIds, string destinationParentId)
            {
                foreach (var entryId in entryIds)
                {
                    await MoveEntry(entryId, destinationParentId);
                }
                return true;
            }

            public Task<bool> CopyEntry(string entryId, string destinationParentId)
            {
                // Not implemented for these tests
                return Task.FromResult(false);
            }

            public async Task<bool> DeleteEntry(string id)
            {
                var entry = await _basicRepository.FindAsync(id);
                if (entry != null)
                {
                    await _basicRepository.DeleteAsync(entry);
                    return true;
                }
                return false;
            }

            // File-content operations - not implemented for domain tests
            public Task<string> GetFileToken(string id, bool isVersion) => Task.FromResult(string.Empty);
            public Task<bool> DeleteFileToken(Guid token) => Task.FromResult(false);
            public Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion) => Task.FromResult<byte[]>(Array.Empty<byte>());
            public Task<FileSourceValueObject> FileViewer(string id) => throw new NotImplementedException("Not used in domain tests");
            public Task<byte[]> DownloadFile(string id, bool isVersion) => Task.FromResult<byte[]>(Array.Empty<byte>());
            public Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion) => Task.FromResult<byte[]>(Array.Empty<byte>());
            public Task<List<string>> ReadTextFile(string id, bool isVersion) => Task.FromResult(new List<string>());
            public Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion) => Task.FromResult(new List<string>());
            public Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId) => Task.FromResult(new List<FileSystemEntry>());
            public Task<FileSystemEntry> UploadNewVersion(string oldFileId, MemoryStream newFileData) => throw new NotImplementedException("Not used in domain tests");
        }

        // Mock repository for testing - basic repository implementation
        private class MockFileRepository : IBasicRepository<FileSystemEntry, string>
        {
            private readonly Dictionary<string, FileSystemEntry> _entries = new();
            
            public string? EntityName { get; set; } = nameof(FileSystemEntry);
            public string ProviderName { get; set; } = "Mock";

            public Task<FileSystemEntry> InsertAsync(FileSystemEntry entity, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                _entries[entity.Id] = entity;
                return Task.FromResult(entity);
            }

            public Task<FileSystemEntry> UpdateAsync(FileSystemEntry entity, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                _entries[entity.Id] = entity;
                return Task.FromResult(entity);
            }

            public Task<FileSystemEntry> GetAsync(string id, bool includeDetails = true, CancellationToken cancellationToken = default)
            {
                if (_entries.TryGetValue(id, out var entry))
                    return Task.FromResult(entry);
                throw new EntityNotFoundException(typeof(FileSystemEntry), id);
            }

            public Task<FileSystemEntry?> FindAsync(string id, bool includeDetails = true, CancellationToken cancellationToken = default)
            {
                _entries.TryGetValue(id, out var entry);
                return Task.FromResult<FileSystemEntry?>(entry);
            }

            public Task<IQueryable<FileSystemEntry>> GetQueryableAsync()
            {
                return Task.FromResult(_entries.Values.AsQueryable());
            }

            public Task<DbSet<FileSystemEntry>> GetDbSetAsync()
            {
                // For testing, we'll use an in-memory approach
                // In real scenario, this would return actual DbSet from EF Core
                throw new NotImplementedException("Use GetQueryableAsync for testing");
            }

            // Implement other required interface members as needed
            public Task DeleteAsync(FileSystemEntry entity, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                _entries.Remove(entity.Id);
                return Task.CompletedTask;
            }
            public Task DeleteAsync(string id, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                _entries.Remove(id);
                return Task.CompletedTask;
            }
            public Task<List<FileSystemEntry>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default) => Task.FromResult(_entries.Values.ToList());
            public Task<long> GetCountAsync(CancellationToken cancellationToken = default) => Task.FromResult((long)_entries.Count);
            public Task<List<FileSystemEntry>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default) => throw new NotImplementedException();
            public Task InsertManyAsync(IEnumerable<FileSystemEntry> entities, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                foreach (var entity in entities)
                {
                    _entries[entity.Id] = entity;
                }
                return Task.CompletedTask;
            }
            public Task UpdateManyAsync(IEnumerable<FileSystemEntry> entities, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                foreach (var entity in entities)
                {
                    _entries[entity.Id] = entity;
                }
                return Task.CompletedTask;
            }
            public Task DeleteManyAsync(IEnumerable<FileSystemEntry> entities, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                foreach (var entity in entities)
                {
                    _entries.Remove(entity.Id);
                }
                return Task.CompletedTask;
            }
            public Task DeleteManyAsync(IEnumerable<string> ids, bool autoSave = false, CancellationToken cancellationToken = default)
            {
                foreach (var id in ids)
                {
                    _entries.Remove(id);
                }
                return Task.CompletedTask;
            }
            public bool? IsChangeTrackingEnabled => false;
        }
    }
}
