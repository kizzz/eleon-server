using Common.Module.Constants;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.BackgroundJobs.Module.Entities
{
  [DisableAuditing]
  public class BackgroundJobMessageEntity : Entity<Guid>, IHasCreationTime, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
    public string TextMessage { get; set; }
    public BackgroundJobMessageType MessageType { get; set; }

    public BackgroundJobMessageEntity()
    {
    }

    public BackgroundJobMessageEntity(Guid id) : base(id)
    {
    }
  }
}
