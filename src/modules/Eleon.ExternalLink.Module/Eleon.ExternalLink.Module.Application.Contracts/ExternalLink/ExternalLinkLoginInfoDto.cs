using Common.Module.Constants;

namespace VPortal.ExternalLink.Module.FileExternalLink
{
  public class ExternalLinkLoginInfoDto
  {
    public string PublicParams { get; set; }
    public string DocumentType { get; set; }
    public ExternalLinkLoginType? LoginType { get; set; }
    public LinkShareStatus Status { get; set; }
  }
}
