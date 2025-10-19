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
        public string LocationName { get; set; } = string.Empty;
    }

    public static class WeatherService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ApiUrl = "https://api.open-meteo.com/v1/forecast";
        private const string GeocodingUrl = "https://geocoding-api.open-meteo.com/v1/search";

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
                // Catch all exceptions and return null to indicate location could not be obtained.
                // This may occur due to denied permissions, unavailable location, or other errors.
                return null;
            }
        }

        public static async Task<string> GetLocationNameAsync(double latitude, double longitude)
        {
            try
            {
                // Use reverse geocoding to get location name
                var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}&zoom=10";
                httpClient.DefaultRequestHeaders.UserAgent.Clear();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MySchool/1.0");
                
                var response = await httpClient.GetStringAsync(url);
                var json = JsonDocument.Parse(response);
                
                var address = json.RootElement.GetProperty("address");
                
                string locationName = "Unknown Location";
                
                // Try to get city, town, or village
                if (address.TryGetProperty("city", out var city))
                    locationName = city.GetString() ?? "Unknown Location";
                else if (address.TryGetProperty("town", out var town))
                    locationName = town.GetString() ?? "Unknown Location";
                else if (address.TryGetProperty("village", out var village))
                    locationName = village.GetString() ?? "Unknown Location";
                else if (address.TryGetProperty("suburb", out var suburb))
                    locationName = suburb.GetString() ?? "Unknown Location";
                
                // Clean up common suffixes
                locationName = locationName
                    .Replace(" City", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(" Council", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(" Municipality", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(" Region", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                
                return locationName;
            }
            catch
            {
                return "Unknown Location";
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
                
                // Get location name
                var locationName = await GetLocationNameAsync(latitude, longitude);

                return new WeatherData
                {
                    Temperature = temperature,
                    WeatherCode = weatherCode,
                    Description = description,
                    Condition = condition,
                    LocationName = locationName
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
                var weather = await GetWeatherAsync(savedLocation.Value.latitude, savedLocation.Value.longitude);
                if (weather != null && !string.IsNullOrWhiteSpace(App.CurrentSettings.WeatherLocationName))
                {
                    weather.LocationName = App.CurrentSettings.WeatherLocationName;
                }
                return weather;
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
