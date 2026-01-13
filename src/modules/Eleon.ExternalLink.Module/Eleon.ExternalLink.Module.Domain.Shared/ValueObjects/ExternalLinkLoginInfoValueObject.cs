using Common.Module.Constants;

namespace VPortal.ExternalLink.Module.ValueObjects
{
  public class ExternalLinkLoginInfoValueObject
  {
    public string PublicParams { get; set; }
    public string DocumentType { get; set; }
    public ExternalLinkLoginType? LoginType { get; set; }
    public LinkShareStatus Status { get; set; }
  }
}
