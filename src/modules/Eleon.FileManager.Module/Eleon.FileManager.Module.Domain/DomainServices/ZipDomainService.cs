using Common.Module.ValueObjects;
using Logging.Module;
using System.IO.Compression;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{
  public class FolderValueObject
  {
    public string Name { get; set; }
    public Dictionary<string, byte[]> Files { get; set; }
    public List<FolderValueObject> Children { get; set; } = new List<FolderValueObject>();
  }

  public class ZipDomainService : DomainService
  {
    private readonly IVportalLogger<ZipDomainService> logger;
    public ZipDomainService(IVportalLogger<ZipDomainService> logger)
    {
      this.logger = logger;
    }

    public async Task<byte[]> CreateArchive(List<FolderValueObject> folders, Dictionary<string, byte[]> files)
    {

      byte[] result = null;
      try
      {
        using (MemoryStream archiveStream = new MemoryStream())
        {
          using (ZipArchive archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
          {
            foreach (var file in files)
            {
              string fileName = file.Key;

              ZipArchiveEntry entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

              using (Stream entryStream = entry.Open())
              {
                await entryStream.WriteAsync(file.Value, 0, file.Value.Length);
              }
            }


            foreach (var folder in folders)
            {
              await AddFolderToArchive(archive, folder, files, string.Empty);
            }
          }

          result = archiveStream.ToArray();
        }
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


    private async Task AddFolderToArchive(ZipArchive archive, FolderValueObject folder, Dictionary<string, byte[]> files, string basePath)
    {
      string folderPath = Path.Combine(basePath, folder.Name);

      foreach (var file in folder.Files)
      {
        string fileName = Path.Combine(folderPath, file.Key);

        ZipArchiveEntry entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

        using (Stream entryStream = entry.Open())
        {
          await entryStream.WriteAsync(file.Value, 0, file.Value.Length);
        }
      }

      foreach (var childFolder in folder.Children)
      {
        await AddFolderToArchive(archive, childFolder, files, folderPath);
      }
    }
    public Task<byte[]> CreateArchive(Dictionary<string, byte[]> files)
    {
      byte[] result = null;
      try
      {
        using (MemoryStream ms = new MemoryStream())
        {
          using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Update))
          {
            foreach (var file in files)
            {
              ZipArchiveEntry entry = archive.CreateEntry(file.Key);
              using (BinaryWriter bw = new BinaryWriter(entry.Open()))
              {
                bw.Write(file.Value);
              }
            }
          }
          result = ms.ToArray();
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return Task.FromResult(result);
    }
  }

}
