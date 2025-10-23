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

        public static async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                // https://docs.velopack.io/reference/cs/Velopack/Sources/GithubSource/constructors
                // GithubSource(string repoUrl, string accessToken, bool prerelease, IFileDownloader downloader = null);
                var mgr = new UpdateManager(new GithubSource("https://github.com/HSP-Studios/MySchool", null, true, null));
                var newVersion = await mgr.CheckForUpdatesAsync();
                return newVersion;
            }
            catch (System.Net.WebException webEx)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed (network): {webEx.Message}");
                throw new Exception("Unable to connect to the update server. Please check your internet connection and try again.", webEx);
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed (HTTP): {httpEx.Message}");
                throw new Exception("Unable to reach the update server. Please check your internet connection and try again.", httpEx);
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Update check timed out");
                throw new Exception("The update check timed out. Please check your internet connection and try again.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
                throw new Exception($"Failed to check for updates: {ex.Message}", ex);
            }
        }

        public static async Task<bool> DownloadAndApplyUpdateAsync(UpdateInfo updateInfo)
        {
            try
            {
                var mgr = new UpdateManager(new GithubSource("https://github.com/HSP-Studios/MySchool", null, true, null));

                // download new version
                await mgr.DownloadUpdatesAsync(updateInfo);

                // install new version and restart app
                mgr.ApplyUpdatesAndRestart(updateInfo);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update download/apply failed: {ex.Message}");
                return false;
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
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

                // Check for updates on startup
                await CheckForUpdatesOnStartup();
            }
            catch
            {
                // Ignore any IO exceptions on startup; app can still run without the folder
            }
        }

        private async Task CheckForUpdatesOnStartup()
        {
            try
            {
                var updateInfo = await CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    var result = MessageBox.Show(
                      $"A new version of MySchool is available!\n\n" +
                      $"Current Version: {updateInfo.TargetFullRelease.Version}\n\n" +
                      $"Would you like to download and install the update now?\n" +
                      $"The application will restart after the update.",
                      "Update Available",
                      MessageBoxButton.YesNo,
                      MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = await DownloadAndApplyUpdateAsync(updateInfo);
                        if (!success)
                        {
                            MessageBox.Show(
                                "Failed to download or apply the update. Please try again later or download manually from GitHub.",
                                "Update Failed",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Startup update check error: {ex.Message}");
                // Silently fail - don't bother user with update check errors on startup
            }
        }
    }
}
