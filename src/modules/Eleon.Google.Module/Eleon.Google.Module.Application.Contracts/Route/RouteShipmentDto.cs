namespace Messaging.Module.ETO.Google
{
  public class RouteShipmentDto
  {
    public string ShipmentType { get; set; }
    public string Label { get; set; }
    public List<RouteVisitRequestDto> Pickups { get; set; } = new List<RouteVisitRequestDto>();
    public List<RouteVisitRequestDto> Deliveries { get; set; } = new List<RouteVisitRequestDto>();
  }
}
