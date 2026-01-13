using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.Google.Module.Configuration;
using File = Google.Apis.Drive.v3.Data.File;

namespace VPortal.Google.Module.DomainServices
{
  public class GoogleDriveDomainService : DomainService
  {
    private readonly IConfiguration configuration;
    private readonly IVportalLogger<GoogleDriveDomainService> logger;
    private readonly GoogleKeyProvider keyProvider;

    public GoogleDriveDomainService(
        IConfiguration configuration,
        IVportalLogger<GoogleDriveDomainService> logger,
        GoogleKeyProvider keyProvider)
    {
      this.configuration = configuration;
      this.logger = logger;
      this.keyProvider = keyProvider;
    }

    public async Task<string> Upload(string name, Stream fileContent, string contentType, string mimeType)
    {

      string result = null;
      try
      {
        var fileMetadata = new File()
        {
          Name = name,
          MimeType = mimeType,
        };

        var service = await GetDriveService();

        string uploadedFileId;
        await using (var fsSource = fileContent)
        {
          var request = service.Files.Create(fileMetadata, fsSource, contentType);
          request.Fields = "*";
          var results = await request.UploadAsync(CancellationToken.None);

          if (results.Status == UploadStatus.Failed)
          {
            Console.WriteLine($"Error uploading file: {results.Exception.Message}");
          }

          uploadedFileId = request.ResponseBody?.Id;
          Console.WriteLine("File ID: " + uploadedFileId);
          return uploadedFileId;
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

    public async Task Delete(string fileId)
    {
      var service = await GetDriveService();

      FilesResource.DeleteRequest deleteRequest = new FilesResource.DeleteRequest(service, fileId);

      await deleteRequest.ExecuteAsync();
    }

    public async Task<MemoryStream> Download(string fileId, string contentType)
    {
      var service = await GetDriveService();

      var request = service.Files.Export(fileId, contentType);

      var stream = new MemoryStream();

      //request.MediaDownloader.ProgressChanged +=
      //    progress =>
      //    {
      //        switch (progress.Status)
      //        {
      //            case DownloadStatus.Downloading:
      //                {
      //                    Console.WriteLine(progress.BytesDownloaded);
      //                    break;
      //                }
      //            case DownloadStatus.Completed:
      //                {
      //                    Console.WriteLine("Download complete.");
      //                    break;
      //                }
      //            case DownloadStatus.Failed:
      //                {
      //                    Console.WriteLine("Download failed.");
      //                    break;
      //                }
      //        }
      //    };
      //request.Download(stream);

      request.ExecuteAsStream().CopyTo(stream);
      return stream;
    }

    public async Task<string> CreateLink(string fileId, string fileViewPermissionType)
    {

      string result = null;
      try
      {
        var service = await GetDriveService();


        var permission = new Permission()
        {
          Type = "anyone",
          Role = fileViewPermissionType,
        };

        PermissionsResource.CreateRequest createRequest = new PermissionsResource.CreateRequest(service, permission, fileId);
        await createRequest.ExecuteAsync();

        FilesResource.GetRequest getRequest = new FilesResource.GetRequest(service, fileId);
        getRequest.Fields = "webViewLink";
        var response = await getRequest.ExecuteAsync();

        return response.WebViewLink;
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

    private async Task<DriveService> GetDriveService()
    {
      string key = await keyProvider.GetDriveKey();
      var serviceAccountCredential = CredentialFactory.FromJson<ServiceAccountCredential>(key);
      var credential = GoogleCredential
          .FromServiceAccountCredential(serviceAccountCredential)
          .CreateScoped(DriveService.ScopeConstants.Drive);

      var service = new DriveService(new BaseClientService.Initializer
      {
        HttpClientInitializer = credential,
        ApplicationName = "Drive API Snippets",
      });

      return service;
    }
  }
}
