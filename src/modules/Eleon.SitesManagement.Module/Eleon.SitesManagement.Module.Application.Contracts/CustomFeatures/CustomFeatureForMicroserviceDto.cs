using VPortal.SitesManagement.Module.CustomFeatures;

namespace VPortal.SitesManagement.Module.Application.Contracts.CustomFeatures
{
  public class CustomFeatureForMicroserviceDto
  {
    public List<CustomFeatureGroupDto> Groups { get; set; }
    public List<CustomFeatureDto> Features { get; set; }
  }
}


