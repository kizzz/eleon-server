using System;
using System.Threading;
using System.Threading.Tasks;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using NSubstitute;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Repositories;

namespace VPortal.FileManager.Module.Tests.TestHelpers;

public static class FileManagerMockHelpers
{
    public static void SetupFileSystemEntryRepositoryGetEntryById(
        IFileSystemEntryRepository repository,
        string id,
        FileSystemEntry entry)
    {
        repository.GetEntryById(id).Returns(entry);
    }

    public static void SetupFileSystemEntryRepositoryGetEntriesByParentId(
        IFileSystemEntryRepository repository,
        string parentId,
        System.Collections.Generic.List<FileSystemEntry> entries,
        EntryKind? kind = null,
        bool recursive = false)
    {
        repository.GetEntriesByParentId(parentId, kind, recursive)
            .Returns(entries);
    }

    public static void SetupFileSystemEntryRepositoryCreateEntry(
        IFileSystemEntryRepository repository,
        FileSystemEntry entry)
    {
        repository.CreateEntry(
            Arg.Any<EntryKind>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>())
            .Returns(entry);
    }

    public static void SetupArchiveRepositoryGetAsync(
        IArchiveRepository repository,
        Guid archiveId,
        FileArchiveEntity archive)
    {
        repository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
    }

    public static void SetupArchiveRepositoryFindAsync(
        IArchiveRepository repository,
        Guid? archiveId,
        FileArchiveEntity? archive)
    {
        repository.FindAsync(archiveId ?? Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(archive);
    }

    public static void SetupFileFactoryGet(
        IFileFactory factory,
        FileArchiveEntity archive,
        FileManagerType fileManagerType,
        IFileSystemEntryRepository repository)
    {
        factory.Get(archive, fileManagerType).Returns(repository);
    }

    // Note: IFileRepository doesn't have GetEntryById, use IFileSystemEntryRepository instead
}
