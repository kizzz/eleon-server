using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using VPortal.SitesManagement.Module.Entities;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Shared.ValueObjects;
public class SiteSettingsValueObject
{
  public Guid SiteId { get; set; }
  public string SiteName { get; set; }
  public Guid? TenantId { get; set; }
  public List<ApplicationEntity> Applications { get; set; } = new List<ApplicationEntity>();
  // public List<TenantHostnameEntity> Hostnames { get; set; } = new List<TenantHostnameEntity>();
}


