using Logging.Module;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{

  public class TemporaryFileDomainService : DomainService
  {
    private readonly IVportalLogger<TemporaryFileDomainService> logger;

    public TemporaryFileDomainService(IVportalLogger<TemporaryFileDomainService> logger)
    {
      this.logger = logger;
    }

    public async Task<string> CreateTempFile(string content, string extension = null)
    {
      string result = null;
      try
      {
        var file = GetTempFile(extension);
        await File.WriteAllTextAsync(file, content);
        result = file;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<string> CreateTempFile(byte[] content, string extension = null)
    {
      string result = null;
      try
      {
        var file = GetTempFile(extension);
        await File.WriteAllBytesAsync(file, content);
        result = file;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task ClearTempFile(string path)
    {
      try
      {
        File.Delete(path);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }

    private string GetTempFile(string extension)
    {
      string path = Path.GetTempPath();
      string filename =
          GuidGenerator.Create().ToString()
          + (extension.IsNullOrWhiteSpace() ? string.Empty : $".{extension}");

      return Path.Combine(path, filename);
    }
  }
}
