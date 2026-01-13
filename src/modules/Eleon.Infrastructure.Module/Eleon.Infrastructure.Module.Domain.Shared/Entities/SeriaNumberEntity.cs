using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Infrastructure.Module.Entities
{
  /// <summary>
  /// Contains information about a seria along with its latest value.
  /// A seria is identified by the DocumentObjectType + RefId pair.
  /// RefId being null is a normal situation, and null is treated as a specific value.
  /// </summary>
  public class SeriaNumberEntity : AggregateRoot<Guid>, IMultiTenant
  {
    public string Prefix { get; set; }
    public string ObjectType { get; set; }
    public string RefId { get; set; }
    public long LastUsedNumber { get; set; }
    public Guid? TenantId { get; set; }

    protected SeriaNumberEntity() { }

    public SeriaNumberEntity(
        Guid id,
        string prefix,
        string objectType,
        long defaultNumber)
    {
      Id = id;
      Prefix = prefix;
      ObjectType = objectType;
      LastUsedNumber = defaultNumber;
    }
  }
}
