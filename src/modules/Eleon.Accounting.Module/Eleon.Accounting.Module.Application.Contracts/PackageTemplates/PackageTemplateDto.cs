using Common.Module.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.Accounting.Module.PackageTemplates
{
  public class PackageTemplateDto
  {
    public Guid Id { get; set; }
    public string PackageName { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public BillingPeriodType BillingPeriodType { get; set; }
    public List<PackageTemplateModuleDto> PackageTemplateModules { get; set; }
    public PackageType PackageType { get; set; }
    public string Description { get; set; }
    public int MaxMembers { get; set; }
    public decimal Price { get; set; }
    public string SystemCurrency { get; set; }
  }
}
