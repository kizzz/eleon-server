using Common.Module.Constants;

namespace VPortal.FileManager.Module.FileExternalLink
{
  public class FileExternalLinkReviewerInfoDto
  {
    public string FileName { get; set; }
    public FileReviewerType ReviewerType { get; set; }
    public LinkShareStatus ReviewerStatus { get; set; }
  }
}
