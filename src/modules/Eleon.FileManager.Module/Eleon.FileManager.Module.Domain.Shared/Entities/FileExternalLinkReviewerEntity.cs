using Common.Module.Constants;
using Messaging.Module.ETO;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileExternalLinkReviewerEntity : Entity<Guid>, IMultiTenant
  {
    public DateTime ExpirationDateTime { get; set; }
    public FileReviewerType ReviewerType { get; set; }
    public string ReviewerKey { get; set; }
    [NotMapped]
    public string ReviewerKeyLabel { get; set; }
    public DateTime LastReviewDates { get; set; }
    public string ExternalLinkCode { get; set; }
    [NotMapped]
    public string Url { get; set; }
    [NotMapped]
    public ExternalLinkEto? ExternalLink { get; set; }
    public LinkShareStatus ReviewerStatus { get; set; }
    public Guid? TenantId { get; set; }

    public FileExternalLinkReviewerEntity()
    {
    }
    public FileExternalLinkReviewerEntity(Guid id) : base(id)
    {
    }
  }
}
