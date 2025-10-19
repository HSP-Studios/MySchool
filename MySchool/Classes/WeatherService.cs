using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MySchool.Classes
{
    public class WeatherData
    {
        public string Description { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty; // Clear, Clouds, Rain, Snow, etc.
    }

    public static class WeatherService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ApiKey = "YOUR_OPENWEATHERMAP_API_KEY"; // User needs to add their API key
        private const string ApiUrl = "https://api.openweathermap.org/data/2.5/weather";

        public static async Task<(double latitude, double longitude)?> GetLocationAsync()
        {
            try
            {
                var geolocator = new Geolocator();
                
                // Request access to location
                var accessStatus = await Geolocator.RequestAccessAsync();
                
                if (accessStatus != GeolocationAccessStatus.Allowed)
                {
                    return null;
                }

                // Get current position
                var position = await geolocator.GetGeopositionAsync();
                
                return (position.Coordinate.Point.Position.Latitude, 
                        position.Coordinate.Point.Position.Longitude);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<WeatherData?> GetWeatherAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"{ApiUrl}?lat={latitude}&lon={longitude}&units=metric&appid={ApiKey}";
                var response = await httpClient.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                var root = json.RootElement;
                var weather = root.GetProperty("weather")[0];
                var main = root.GetProperty("main");

                return new WeatherData
                {
                    Description = weather.GetProperty("description").GetString() ?? string.Empty,
                    Temperature = main.GetProperty("temp").GetDouble(),
                    Icon = weather.GetProperty("icon").GetString() ?? string.Empty,
                    Condition = weather.GetProperty("main").GetString() ?? string.Empty
                };
            }
            catch
            {
                return null;
            }
        }

        public static async Task<WeatherData?> GetCurrentWeatherAsync()
        {
            var location = await GetLocationAsync();
            if (location == null)
                return null;

            return await GetWeatherAsync(location.Value.latitude, location.Value.longitude);
        }
    }
}
