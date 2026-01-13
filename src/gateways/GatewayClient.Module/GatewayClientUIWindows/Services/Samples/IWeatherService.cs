namespace VPortal.GatewayClient.UI.Windows.Services.Samples;

public interface IWeatherService
{
    Task<IEnumerable<Location>> GetLocations(string query);
    Task<WeatherResponse> GetWeather(Coordinate location);
}
