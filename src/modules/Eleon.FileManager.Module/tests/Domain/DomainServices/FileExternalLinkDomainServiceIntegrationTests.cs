using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

// Integration tests for FileExternalLinkDomainService
public class FileExternalLinkDomainServiceIntegrationTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileExternalLinkDomainService GetService()
    {
        return GetRequiredService<FileExternalLinkDomainService>();
    }

    [Fact]
    public async Task GetLinksAsync_WithShareStatuses_ReturnsFilteredLinks()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileShareStatuses = new List<FileShareStatus> { FileShareStatus.Readonly, FileShareStatus.Modify };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetLinksAsync(archiveId, fileShareStatuses);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<VPortal.FileManager.Module.Entities.FileExternalLinkEntity>>();
    }

    [Fact]
    public async Task GetLinksAsync_EmptyStatusList_ReturnsEmptyList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileShareStatuses = new List<FileShareStatus>();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetLinksAsync(archiveId, fileShareStatuses);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
