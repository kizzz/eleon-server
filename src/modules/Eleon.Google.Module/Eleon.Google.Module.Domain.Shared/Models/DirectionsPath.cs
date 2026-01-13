using System.Collections.Generic;

namespace VPortal.Google.Module.Models
{
  public class DirectionsPath
  {
    public List<LatLng> Path { get; set; }
    public int DurationHours { get; set; }
    public int DurationMinutes { get; set; }
    public int DurationSeconds { get; set; }
    public double DistanceKilometers { get; set; }
  }
}
