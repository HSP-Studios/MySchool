using System;
using System.IO;
using System.Text.Json;

namespace MySchool
{
    public class UserSettings
    {
        public bool IsDarkMode { get; set; } = false;
        public (double latitude, double longitude)? WeatherLocation { get; set; } = null;
        public string WeatherLocationName { get; set; } = string.Empty;
        
        // Developer Mode Settings
        public bool DeveloperMode { get; set; } = false;
        public string ForceLayoutMode { get; set; } = "Auto"; // Auto, Weekend, Weekday
        public bool ForceWeatherEnabled { get; set; } = false;
        public string ForcedWeatherCondition { get; set; } = "Clear"; // Clear, Clouds, Rain, Snow, Thunderstorm, Drizzle
        public double ForcedTemperature { get; set; } = 23.0;
        public string ForcedWeatherDescription { get; set; } = "Clear sky";
        public string ForcedLocationName { get; set; } = "Test Location";
    }

    public static class SettingsService
    {
        private const string SettingsFileName = "user_settings.json";

        private static string GetSettingsPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mySchoolPath = Path.Combine(appData, "MySchool");
            return Path.Combine(mySchoolPath, SettingsFileName);
        }

        public static UserSettings Load()
        {
            try
            {
                var path = GetSettingsPath();
                if (!File.Exists(path))
                {
                    // Save defaults on first run
                    var defaults = new UserSettings();
                    Save(defaults);
                    return defaults;
                }

                var json = File.ReadAllText(path);
                var settings = JsonSerializer.Deserialize<UserSettings>(json);
                return settings ?? new UserSettings();
            }
            catch
            {
                return new UserSettings();
            }
        }

        public static void Save(UserSettings settings)
        {
            try
            {
                var path = GetSettingsPath();
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch
            {
                // ignore IO errors
            }
        }
    }
}
