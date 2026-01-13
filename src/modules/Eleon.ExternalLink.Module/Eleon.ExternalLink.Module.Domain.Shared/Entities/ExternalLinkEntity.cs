using Common.Module.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.ExternalLink.Module.Entities
{
  public class ExternalLinkEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public DateTime ExpirationDateTime { get; set; }
    public string PublicParams { get; set; }
    public string PrivateParams { get; set; }
    public ExternalLinkLoginType LoginType { get; set; }
    public string DocumentType { get; set; }
    public string LoginKey { get; set; }
    [NotMapped]
    public string LoginKeyLabel { get; set; }
    public int LoginAttempts { get; set; }
    public DateTime? LastLoginSuccessDate { get; set; }
    public DateTime? LastLoginAttemptDate { get; set; }
    public DateTime? LastPublicRequestDate { get; set; }
    public LinkShareStatus Status { get; set; }
    public bool IsOneTimeLink { get; set; }
    public string ExternalLinkCode { get; set; }
    public string ExternalLinkUrl { get; set; }

    public string FullLink
    {
      get
      {
        return (ExternalLinkUrl ?? string.Empty)
            .Replace("{link}", ExternalLinkCode);
      }
    }
    public Guid? TenantId { get; set; }

    protected ExternalLinkEntity()
    {
    }

    public ExternalLinkEntity(Guid id) : base(id)
    {
    }
  }
}
