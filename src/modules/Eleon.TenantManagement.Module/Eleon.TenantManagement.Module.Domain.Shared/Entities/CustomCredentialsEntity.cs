using Common.Module.Constants;
using Eleon.TenantManagement.Module.Eleon.TenantManagement.Module.Domain.Shared.Consts;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Identity.Module.Entities
{
  public class CustomCredentialsEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }

    public Guid UserId { get; set; }

    public string Value { get; set; }

    public CustomCredentialsSet CredentialsSet { get; set; }

    public string Claims { get; set; }

    public CustomCredentialsEntity(Guid id, Guid userId, CustomCredentialsSet credentialsSet, string value)
    {
      Id = id;
      UserId = userId;
      CredentialsSet = credentialsSet;
      Value = value;
    }
  }
}
