using System;
using VPortal.SitesManagement.Module.Consts;

namespace VPortal.SitesManagement.Module.ApplicationMenuItems
{
  public class ApplicationMenuItemDto
  {
    public Guid Id { get; set; }
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
  }
}


