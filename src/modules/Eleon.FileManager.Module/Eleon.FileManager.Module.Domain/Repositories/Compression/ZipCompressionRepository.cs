using Common.Module.ValueObjects;
using Logging.Module;
using Volo.Abp.DependencyInjection;
using VPortal.FileManager.Module.ValueObjects;
using VPortal.Infrastructure.Module.Domain.DomainServices;

namespace VPortal.FileManager.Module.Repositories.Compression
{
  public class ZipCompressionRepository : ICompressionRepository, ITransientDependency
  {
    private readonly IVportalLogger<ZipCompressionRepository> logger;
    private readonly ZipDomainService zipDomainService;

    public ZipCompressionRepository(
        IVportalLogger<ZipCompressionRepository> logger,
        ZipDomainService zipDomainService)
    {
      this.logger = logger;
      this.zipDomainService = zipDomainService;
    }
    public async Task<byte[]> Compress(List<HierarchyFolderValueObject> virtualFolders, List<FileValueObject> files)
    {

      byte[] result = null;
      try
      {
        var filePairs = files.ToDictionary(f => f.Name, f => f.Source);
        var folders = ConvertHierarchyFoldersToFolderValueObject(virtualFolders).ToList();
        result = await zipDomainService.CreateArchive(folders, filePairs);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    private IEnumerable<FolderValueObject> ConvertHierarchyFoldersToFolderValueObject(List<HierarchyFolderValueObject> hierarchyFolders)
    {
      foreach (var hierarchyFolder in hierarchyFolders)
      {
        var folder = new FolderValueObject();

        folder.Name = hierarchyFolder.Name;
        folder.Files = hierarchyFolder.Files.ToDictionary(f => f.Name, f => f.Source);
        if (hierarchyFolder.Children.Any())
        {
          folder.Children = ConvertHierarchyFoldersToFolderValueObject(hierarchyFolders).ToList();
        }
        yield return folder;
      }
    }
  }
}
