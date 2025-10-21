using MySchool.Classes;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Windows;
using Velopack;
using Velopack.Sources;

namespace MySchool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserSettings CurrentSettings { get; private set; } = new UserSettings();

        [STAThread]
        private static void Main(string[] args)
        {
            VelopackApp.Build().Run();
            App app = new();
            app.InitializeComponent();
            app.Run();
        }

        private static async Task UpdateApp()
        {
            // https://docs.velopack.io/reference/cs/Velopack/Sources/GithubSource/constructors
            var mgr = new UpdateManager(new GithubSource("https://github.com/HSP-Studios/MySchool", null, false, null));

            // check for new version
            var newVersion = await mgr.CheckForUpdatesAsync();
            if (newVersion == null)
                return; // no update available

            // download new version
            await mgr.DownloadUpdatesAsync(newVersion);

            // install new version and restart app
            mgr.ApplyUpdatesAndRestart(newVersion);
        }

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
