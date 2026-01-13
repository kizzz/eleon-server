using Common.Module.Constants;
using Messaging.Module.ETO;
using System;
using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.FileExternalLink
{
  public class FileExternalLinkReviewerDto : EntityDto<Guid>
  {
    public LinkShareStatus ReviewerStatus { get; set; }
    public DateTime ExpirationDateTime { get; set; }
    public FileReviewerType ReviewerType { get; set; }
    public string ReviewerKey { get; set; }
    public string ReviewerKeyLabel { get; set; }
    public DateTime LastReviewDates { get; set; }
    public string ExternalLinkCode { get; set; }
    public string Url { get; set; }
    public ExternalLinkEto? ExternalLink { get; set; }
  }
}
