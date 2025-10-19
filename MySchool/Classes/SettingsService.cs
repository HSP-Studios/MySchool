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
        public bool IsDeveloperMode { get; set; } = false;
        public bool ForceDayTimeEnabled { get; set; } = false;
        public DayOfWeek ForcedDayOfWeek { get; set; } = DayOfWeek.Monday;
        public TimeSpan ForcedTimeOfDay { get; set; } = new TimeSpan(12, 0, 0); // Default to 12:00 PM
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

        public static string GetDataFolderPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "MySchool");
        }

        public static DateTime GetCurrentDateTime()
        {
            var settings = App.CurrentSettings;
            
            if (settings.IsDeveloperMode && settings.ForceDayTimeEnabled)
            {
                // Create a DateTime with the forced day and time
                var today = DateTime.Today;
                int daysToAdd = ((int)settings.ForcedDayOfWeek - (int)today.DayOfWeek + 7) % 7;
                var forcedDate = today.AddDays(daysToAdd);
                return forcedDate.Add(settings.ForcedTimeOfDay);
            }
            
            return DateTime.Now;
        }

        public static DayOfWeek GetCurrentDayOfWeek()
        {
            return GetCurrentDateTime().DayOfWeek;
        }

        public static TimeSpan GetCurrentTimeOfDay()
        {
            return GetCurrentDateTime().TimeOfDay;
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
