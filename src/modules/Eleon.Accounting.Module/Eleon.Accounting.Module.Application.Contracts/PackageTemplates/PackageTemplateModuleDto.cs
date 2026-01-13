using Common.Module.Constants;
using System;

namespace VPortal.Accounting.Module.PackageTemplates
{
  public class PackageTemplateModuleDto
  {
    public Guid Id { get; set; }
    public Guid PackageTemplateEntityId { get; set; }
    public string Name { get; set; }
    public Guid? RefId { get; set; }
    public PackageModuleType ModuleType { get; set; }
    public string Description { get; set; }
    public string ModuleData { get; set; }
  }
}
