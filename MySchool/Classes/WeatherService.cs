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
        public int WeatherCode { get; set; }
        public string Condition { get; set; } = string.Empty; // Clear, Clouds, Rain, Snow, etc.
    }

    public static class WeatherService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ApiUrl = "https://api.open-meteo.com/v1/forecast";

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
                var url = $"{ApiUrl}?latitude={latitude}&longitude={longitude}&current=temperature_2m,weather_code&temperature_unit=celsius";
                var response = await httpClient.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                var current = json.RootElement.GetProperty("current");
                var temperature = current.GetProperty("temperature_2m").GetDouble();
                var weatherCode = current.GetProperty("weather_code").GetInt32();

                var (description, condition) = GetWeatherDescription(weatherCode);

                return new WeatherData
                {
                    Temperature = temperature,
                    WeatherCode = weatherCode,
                    Description = description,
                    Condition = condition
                };
            }
            catch
            {
                return null;
            }
        }

        public static async Task<WeatherData?> GetCurrentWeatherAsync()
        {
            // Try to get saved location first
            var savedLocation = App.CurrentSettings.WeatherLocation;
            if (savedLocation.HasValue)
            {
                return await GetWeatherAsync(savedLocation.Value.latitude, savedLocation.Value.longitude);
            }

            // Fall back to device location
            var location = await GetLocationAsync();
            if (location == null)
                return null;

            return await GetWeatherAsync(location.Value.latitude, location.Value.longitude);
        }

        private static (string description, string condition) GetWeatherDescription(int code)
        {
            // Based on WMO Weather interpretation codes
            return code switch
            {
                0 => ("Clear sky", "Clear"),
                1 => ("Mainly clear", "Clear"),
                2 => ("Partly cloudy", "Clouds"),
                3 => ("Overcast", "Clouds"),
                45 => ("Foggy", "Clouds"),
                48 => ("Depositing rime fog", "Clouds"),
                51 => ("Light drizzle", "Drizzle"),
                53 => ("Moderate drizzle", "Drizzle"),
                55 => ("Dense drizzle", "Drizzle"),
                56 => ("Light freezing drizzle", "Drizzle"),
                57 => ("Dense freezing drizzle", "Drizzle"),
                61 => ("Slight rain", "Rain"),
                63 => ("Moderate rain", "Rain"),
                65 => ("Heavy rain", "Rain"),
                66 => ("Light freezing rain", "Rain"),
                67 => ("Heavy freezing rain", "Rain"),
                71 => ("Slight snow", "Snow"),
                73 => ("Moderate snow", "Snow"),
                75 => ("Heavy snow", "Snow"),
                77 => ("Snow grains", "Snow"),
                80 => ("Slight rain showers", "Rain"),
                81 => ("Moderate rain showers", "Rain"),
                82 => ("Violent rain showers", "Rain"),
                85 => ("Slight snow showers", "Snow"),
                86 => ("Heavy snow showers", "Snow"),
                95 => ("Thunderstorm", "Thunderstorm"),
                96 => ("Thunderstorm with slight hail", "Thunderstorm"),
                99 => ("Thunderstorm with heavy hail", "Thunderstorm"),
                _ => ("Unknown", "Clear")
            };
        }
    }
}
