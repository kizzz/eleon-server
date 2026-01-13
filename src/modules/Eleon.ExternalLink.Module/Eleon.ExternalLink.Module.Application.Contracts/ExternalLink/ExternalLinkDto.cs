using Common.Module.Constants;
using System;
using Volo.Abp.Application.Dtos;

namespace VPortal.ExternalLink.Module.FileExternalLink
{
  public class ExternalLinkDto : EntityDto<Guid>
  {
    public DateTime ExpirationDateTime { get; set; }
    public string PublicParams { get; set; }
    public string PrivateParams { get; set; }
    public ExternalLinkLoginType? LoginType { get; set; }
    public string DocumentType { get; set; }
    public string LoginKey { get; set; }
    public string LoginKeyLabel { get; set; }
    public int LoginAttempts { get; set; }
    public DateTime? LastLoginSuccessDate { get; set; }
    public DateTime LastLoginAttemptDate { get; set; }
    public DateTime LastPublicRequestDate { get; set; }
    public LinkShareStatus Status { get; set; }
    public bool IsOneTimeLink { get; set; }
    public string ExternalLinkCode { get; set; }
    public string ExternalLinkUrl { get; set; }
    public string FullLink { get; set; }
  }
}
