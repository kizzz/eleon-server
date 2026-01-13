using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using NSubstitute;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

public class FileStatusDomainServiceTests : DomainTestBase
{
    private readonly FileStatusDomainService _service;
    private readonly IFileStatusRepository _repository;

    public FileStatusDomainServiceTests()
    {
        _repository = CreateMockFileStatusRepository();

        _service = new FileStatusDomainService(
            CreateMockLogger<FileStatusDomainService>(),
            _repository);
    }

    [Fact]
    public async Task GetFileStatusAsync_WithStatus_ReturnsStatus()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var status = FileManagerTestDataBuilder.FileStatus()
            .WithArchiveId(archiveId)
            .WithFileId(fileId)
            .WithFileStatus(FileStatus.Trash)
            .Build();

        _repository.GetByFileId(archiveId, fileId)
            .Returns(status);

        // Act
        var result = await _service.GetFileStatusAsync(archiveId, fileId);

        // Assert
        result.Should().Be(FileStatus.Trash);
    }

    [Fact]
    public async Task GetFileStatusAsync_WithoutStatus_ReturnsActive()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        _repository.GetByFileId(archiveId, fileId)
            .Returns((FileStatusEntity)null);

        // Act
        var result = await _service.GetFileStatusAsync(archiveId, fileId);

        // Assert
        result.Should().Be(FileStatus.Active);
    }

    [Fact]
    public async Task GetFolderStatusAsync_WithStatus_ReturnsStatus()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var status = FileManagerTestDataBuilder.FileStatus()
            .WithArchiveId(archiveId)
            .WithFolderId(folderId)
            .WithFileStatus(FileStatus.Trash)
            .Build();

        _repository.GetByFolderId(archiveId, folderId)
            .Returns(status);

        // Act
        var result = await _service.GetFolderStatusAsync(archiveId, folderId);

        // Assert
        result.Should().Be(FileStatus.Trash);
    }

    [Fact]
    public async Task UpdateFileStatus_ValidInput_UpdatesSuccessfully()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var newStatus = FileStatus.Trash;

        _repository.UpdateFileStatus(archiveId, fileId, newStatus)
            .Returns(true);

        // Act
        var result = await _service.UpdateFileStatus(archiveId, fileId, newStatus);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateFolderStatus_ValidInput_UpdatesSuccessfully()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var newStatus = FileStatus.Trash;

        _repository.UpdateFolderStatus(archiveId, folderId, newStatus)
            .Returns(true);

        // Act
        var result = await _service.UpdateFolderStatus(archiveId, folderId, newStatus);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetFileListAsync_WithStatuses_ReturnsFilteredList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileStatuses = new List<FileStatus> { FileStatus.Trash };
        var statusEntities = new List<FileStatusEntity>
        {
            FileManagerTestDataBuilder.FileStatus()
                .WithArchiveId(archiveId)
                .WithFileStatus(FileStatus.Trash)
                .Build()
        };

        _repository.GetFileListAsync(archiveId, fileStatuses)
            .Returns(statusEntities);

        // Act
        var result = await _service.GetFileListAsync(archiveId, fileStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetFileListAsync_EmptyStatusList_ReturnsEmptyList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileStatuses = new List<FileStatus>();

        _repository.GetFileListAsync(archiveId, fileStatuses)
            .Returns(new List<FileStatusEntity>());

        // Act
        var result = await _service.GetFileListAsync(archiveId, fileStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFileListAsync_MultipleStatuses_ReturnsFilteredList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileStatuses = new List<FileStatus> { FileStatus.Trash, FileStatus.Archived };
        var statusEntities = new List<FileStatusEntity>
        {
            FileManagerTestDataBuilder.FileStatus()
                .WithArchiveId(archiveId)
                .WithFileStatus(FileStatus.Trash)
                .Build(),
            FileManagerTestDataBuilder.FileStatus()
                .WithArchiveId(archiveId)
                .WithFileStatus(FileStatus.Archived)
                .Build()
        };

        _repository.GetFileListAsync(archiveId, fileStatuses)
            .Returns(statusEntities);

        // Act
        var result = await _service.GetFileListAsync(archiveId, fileStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.FileStatus == FileStatus.Trash);
        result.Should().Contain(s => s.FileStatus == FileStatus.Archived);
    }

    [Fact]
    public async Task GetFolderListAsync_WithStatuses_ReturnsFilteredList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileStatuses = new List<FileStatus> { FileStatus.Trash };
        var statusEntities = new List<FileStatusEntity>
        {
            FileManagerTestDataBuilder.FileStatus()
                .WithArchiveId(archiveId)
                .WithFolderId(Guid.NewGuid().ToString())
                .WithFileStatus(FileStatus.Trash)
                .Build()
        };

        _repository.GetFolderListAsync(archiveId, fileStatuses)
            .Returns(statusEntities);

        // Act
        var result = await _service.GetFolderListAsync(archiveId, fileStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateFileStatus_NonExistentFile_ReturnsFalse()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var newStatus = FileStatus.Trash;

        _repository.UpdateFileStatus(archiveId, fileId, newStatus)
            .Returns(false);

        // Act
        var result = await _service.UpdateFileStatus(archiveId, fileId, newStatus);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateFolderStatus_NonExistentFolder_ReturnsFalse()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var newStatus = FileStatus.Trash;

        _repository.UpdateFolderStatus(archiveId, folderId, newStatus)
            .Returns(false);

        // Act
        var result = await _service.UpdateFolderStatus(archiveId, folderId, newStatus);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetFileStatusAsync_ExceptionHandled_ReturnsActive()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        _repository.GetByFileId(archiveId, fileId)
            .Returns<Task<FileStatusEntity>>(callInfo => throw new Exception("Database error"));

        // Act
        var result = await _service.GetFileStatusAsync(archiveId, fileId);

        // Assert - Should return Active as default when exception occurs
        result.Should().Be(FileStatus.Active);
    }

    [Fact]
    public async Task GetFolderStatusAsync_WithoutStatus_ReturnsActive()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();

        _repository.GetByFolderId(archiveId, folderId)
            .Returns((FileStatusEntity)null);

        // Act - The service returns Active as default when status is null
        var result = await _service.GetFolderStatusAsync(archiveId, folderId);

        // Assert
        result.Should().Be(FileStatus.Active);
    }

    [Fact]
    public async Task GetFolderListAsync_EmptyStatusList_ReturnsEmptyList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileStatuses = new List<FileStatus>();

        _repository.GetFolderListAsync(archiveId, fileStatuses)
            .Returns(new List<FileStatusEntity>());

        // Act
        var result = await _service.GetFolderListAsync(archiveId, fileStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FillStatuses_WithFilesAndFolders_FillsStatuses()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();

        var files = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(fileId)
                .WithArchiveId(archiveId)
                .AsFile()
                .Build()
        };

        var folders = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(folderId)
                .WithArchiveId(archiveId)
                .AsFolder()
                .Build()
        };

        var statusEntity = FileManagerTestDataBuilder.FileStatus()
            .WithArchiveId(archiveId)
            .WithFileId(fileId)
            .WithFileStatus(FileStatus.Trash)
            .Build();

        var dbSet = new List<FileStatusEntity> { statusEntity }.AsQueryable().BuildMockDbSet();
        ((IEfCoreRepository<FileStatusEntity, Guid>)_repository).GetDbSetAsync()
            .Returns(Task.FromResult(dbSet));

        // Act
        var result = await _service.FillStatuses(archiveId, files, folders);

        // Assert
        result.Should().BeTrue();
        files[0].Status.Should().Be(FileStatus.Trash);
        folders[0].Status.Should().Be(FileStatus.Active); // No status found, defaults to Active
    }

    [Fact]
    public async Task FillStatuses_NoStatuses_FillsWithActive()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        var files = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(fileId)
                .WithArchiveId(archiveId)
                .AsFile()
                .Build()
        };

        var folders = new List<FileSystemEntry>();

        var dbSet = new List<FileStatusEntity>().AsQueryable().BuildMockDbSet();
        ((IEfCoreRepository<FileStatusEntity, Guid>)_repository).GetDbSetAsync()
            .Returns(Task.FromResult(dbSet));

        // Act
        var result = await _service.FillStatuses(archiveId, files, folders);

        // Assert
        result.Should().BeTrue();
        files[0].Status.Should().Be(FileStatus.Active);
    }

    [Fact]
    public async Task UpdateFileStatus_AllStatusTypes_UpdatesSuccessfully()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        var statuses = new[] { FileStatus.Active, FileStatus.Trash, FileStatus.Archived };

        foreach (var status in statuses)
        {
            _repository.UpdateFileStatus(archiveId, fileId, status)
                .Returns(true);

            // Act
            var result = await _service.UpdateFileStatus(archiveId, fileId, status);

            // Assert
            result.Should().BeTrue();
        }
    }

    [Fact]
    public async Task UpdateFolderStatus_AllStatusTypes_UpdatesSuccessfully()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();

        var statuses = new[] { FileStatus.Active, FileStatus.Trash, FileStatus.Archived };

        foreach (var status in statuses)
        {
            _repository.UpdateFolderStatus(archiveId, folderId, status)
                .Returns(true);

            // Act
            var result = await _service.UpdateFolderStatus(archiveId, folderId, status);

            // Assert
            result.Should().BeTrue();
        }
    }
}
