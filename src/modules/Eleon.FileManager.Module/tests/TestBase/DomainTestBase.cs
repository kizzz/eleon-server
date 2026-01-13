using Eleon.TestsBase.Lib.TestBase;
using NSubstitute;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;
using VPortal.FileManager.Module;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Repositories;

namespace VPortal.FileManager.Module.Tests.TestBase;

public abstract class DomainTestBase : MockingTestBase
{
    protected IFileSystemEntryRepository CreateMockFileSystemEntryRepository()
    {
        return Substitute.For<IFileSystemEntryRepository>();
    }

    protected IFileRepository CreateMockFileRepository()
    {
        return Substitute.For<IFileRepository>();
    }

    protected IArchiveRepository CreateMockArchiveRepository()
    {
        return Substitute.For<IArchiveRepository>();
    }

    protected IFileFactory CreateMockFileFactory()
    {
        return Substitute.For<IFileFactory>();
    }

    protected IFileStatusRepository CreateMockFileStatusRepository()
    {
        return Substitute.For<IFileStatusRepository, IEfCoreRepository<FileStatusEntity, Guid>>();
    }

    protected IFileExternalLinkRepository CreateMockFileExternalLinkRepository()
    {
        return Substitute.For<IFileExternalLinkRepository, IEfCoreRepository<FileExternalLinkEntity, Guid>>();
    }

    protected IPhysicalFolderRepository CreateMockPhysicalFolderRepository()
    {
        return Substitute.For<IPhysicalFolderRepository>();
    }

    protected IFileArchivePermissionRepository CreateMockFileArchivePermissionRepository()
    {
        return Substitute.For<IFileArchivePermissionRepository>();
    }

    protected IFileArchiveFavouriteRepository CreateMockFileArchiveFavouriteRepository()
    {
        return Substitute.For<IFileArchiveFavouriteRepository>();
    }

    protected Volo.Abp.Identity.IOrganizationUnitRepository CreateMockOrganizationUnitRepository()
    {
        return Substitute.For<Volo.Abp.Identity.IOrganizationUnitRepository>();
    }

    protected new IObjectMapper<ModuleDomainModule> CreateMockObjectMapper()
    {
        return CreateMockObjectMapper<ModuleDomainModule>();
    }
}
