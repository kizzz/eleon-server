using System;
using System.Collections.Generic;
using VPortal.SitesManagement.Module.ClientApplications;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class ApplicationModuleDto
  {
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public string PluginName { get; set; }
    public Guid? ParentId { get; set; }
    public string LoadLevel { get; set; }
    public int OrderIndex { get; set; }
    public string Expose { get; set; }
    public List<ClientApplicationPropertyDto> Properties { get; set; }
    public Guid ClientApplicationEntityId { get; set; }
  }
}


