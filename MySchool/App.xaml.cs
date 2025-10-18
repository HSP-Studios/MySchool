using System.Configuration;
using System.Data;
using System.Windows;
using System;
using System.IO;
using System.Text.Json;

namespace MySchool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserSettings CurrentSettings { get; private set; } = new UserSettings();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string mySchoolPath = Path.Combine(appData, "MySchool");
                if (!Directory.Exists(mySchoolPath))
                {
                    Directory.CreateDirectory(mySchoolPath);
                }

                // Load settings and apply theme
                CurrentSettings = SettingsService.Load();
                ThemeManager.ApplyTheme(CurrentSettings.IsDarkMode);
            }
            catch
            {
                // Ignore any IO exceptions on startup; app can still run without the folder
            }
        }
    }

}
