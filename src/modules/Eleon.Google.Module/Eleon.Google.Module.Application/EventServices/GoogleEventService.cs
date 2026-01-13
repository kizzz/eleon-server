using Common.EventBus.Module;
using Commons.Module.Messages.Google;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Google.Module.DomainServices;

namespace GoogleModule.Google.Module.Application.EventServices;

public class GoogleEventService :
        IDistributedEventHandler<GoogleDriveUploadRequestMsg>,
        ITransientDependency
{
  private readonly IVportalLogger<GoogleEventService> _logger;
  private readonly IResponseContext _responseContext;
  private readonly GoogleDriveDomainService _domain;

  public GoogleEventService(
      IVportalLogger<GoogleEventService> logger,
      IResponseContext responseContext,
      GoogleDriveDomainService domain)
  {
    _logger = logger;
    _responseContext = responseContext;
    _domain = domain;
  }

  public async Task HandleEventAsync(GoogleDriveUploadRequestMsg eventData)
  {
    var response = new GoogleDriveUploadResponseMsg();
    try
    {
      await using var ms = new MemoryStream(eventData.Content, writable: false);
      var fileId = await _domain.Upload(eventData.Name, ms, eventData.ContentType, eventData.MimeType);
      response.FileId = fileId;
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
      response.Error = ex.Message;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  public async Task HandleEventAsync(GoogleDriveDeleteRequestMsg eventData)
  {
    var response = new GoogleDriveDeleteResponseMsg();
    try
    {
      await _domain.Delete(eventData.FileId);
      response.Success = true;
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
      response.Success = false;
      response.Error = ex.Message;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  public async Task HandleEventAsync(GoogleDriveDownloadRequestMsg eventData)
  {
    var response = new GoogleDriveDownloadResponseMsg
    {
      FileName = eventData.FileName,
      ContentType = eventData.ContentType
    };

    try
    {
      var stream = await _domain.Download(eventData.FileId, eventData.ContentType);
      try
      {
        stream.Position = 0;
        response.Content = stream.ToArray();
      }
      finally
      {
        await stream.DisposeAsync();
      }
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
      response.Error = ex.Message;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  public async Task HandleEventAsync(GoogleDriveCreateLinkRequestMsg eventData)
  {
    var response = new GoogleDriveCreateLinkResponseMsg();
    try
    {
      var link = await _domain.CreateLink(eventData.FileId, eventData.PermissionRole);
      response.WebViewLink = link;
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
      response.Error = ex.Message;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
