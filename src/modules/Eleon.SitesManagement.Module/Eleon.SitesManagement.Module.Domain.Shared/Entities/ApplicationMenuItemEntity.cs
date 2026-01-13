using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using VPortal.SitesManagement.Module.Consts;

namespace VPortal.SitesManagement.Module.Entities
{
  public class ApplicationMenuItemEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid ApplicationId { get; set; }
    public string Path { get; set; }
    public bool IsUrl { get; set; }
    public bool IsNewWindow { get; set; }
    public string Label { get; set; }
    public string ParentName { get; set; }
    public string Icon { get; set; }
    public int Order { get; set; }
    public string RequiredPolicy { get; set; }
    public MenuType MenuType { get; set; }
    public ItemType ItemType { get; set; }
    public bool Display { get; set; }

    public ApplicationMenuItemEntity(Guid id)
    {
      Id = id;
    }

    protected ApplicationMenuItemEntity() { }
  }
}


