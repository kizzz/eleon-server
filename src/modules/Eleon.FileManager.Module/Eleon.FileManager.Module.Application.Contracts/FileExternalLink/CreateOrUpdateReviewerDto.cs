using Messaging.Module.ETO;
using System;

namespace VPortal.FileManager.Module.FileExternalLink
{
  public class CreateOrUpdateReviewerDto
  {
    public Guid FileExternalLinkId { get; set; }
    public FileExternalLinkReviewerDto UpdatedReviewer { get; set; }
    public ExternalLinkEto? ExternalLink { get; set; }
  }
}
