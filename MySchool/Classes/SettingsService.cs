using System.IO;
using System.Text.Json;

namespace MySchool.Classes
{
    public class UserSettings
    {
        [Obsolete("Use ThemeName instead")]
        public bool IsDarkMode { get; set; } = false;
        
        public string ThemeName { get; set; } = "Light";
        public (double latitude, double longitude)? WeatherLocation { get; set; } = null;
        public string WeatherLocationName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
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
    
                if (settings != null)
                {
                    // Migration: If ThemeName is not set but IsDarkMode is true, migrate to "Dark"
                    if (string.IsNullOrWhiteSpace(settings.ThemeName))
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        settings.ThemeName = settings.IsDarkMode ? "Dark" : "Light";
#pragma warning restore CS0618 // Type or member is obsolete
                        Save(settings); // Save migrated settings
                    }
                    return settings;
                }
 
                return new UserSettings();
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
