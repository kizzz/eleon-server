using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module.CustomFeatures;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.CustomFeatures
{
  public class CustomFeaturesForMicroserviceDto
  {
    public Guid ServiceId { get; set; }
    public List<CustomFeatureGroupDto> Groups { get; set; }
    public List<CustomFeatureDto> Features { get; set; }
  }
}


