using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Core.Infrastructure.Module.Entities
{
  public class DashboardSettingEntity : AggregateRoot<Guid>, IHasCreationTime, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public int? XCoordinate { get; set; }
    public int? YCoordinate { get; set; }
    public int? Cols { get; set; }
    public int? MaxItemCols { get; set; }
    public int? MinItemCols { get; set; }
    public int? Rows { get; set; }
    public int? MaxItemRows { get; set; }
    public int? MinItemRows { get; set; }
    public string Label { get; set; }
    public string Template { get; set; }
    public DateTime CreationTime { get; set; }
    public bool DragEnabled { get; set; }
    public bool ResizeEnabled { get; set; }
    public bool CompactEnabled { get; set; }
    public Guid UserId { get; set; }
    public bool IsDefault { get; set; }

    public DashboardSettingEntity()
    {

    }

    public DashboardSettingEntity(Guid id) : base(id)
    {

    }
  }
}
