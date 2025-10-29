using MySchool.Classes;
using System.IO;
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
            Logger.Info("Updates", "Starting update check...");

            try
            {
                Logger.Info("Updates", "Initializing UpdateManager with GitHub source");
                Logger.Debug("Updates", "Repository: https://github.com/HSP-Studios/MySchool, Prerelease: true");

                // https://docs.velopack.io/reference/cs/Velopack/Sources/GithubSource/constructors
                // GithubSource(string repoUrl, string accessToken, bool prerelease, IFileDownloader downloader = null);
                var mgr = new UpdateManager(new GithubSource("https://github.com/HSP-Studios/MySchool", null, true, null));

                Logger.Info("Updates", "Checking for available updates from GitHub...");
                var newVersion = await mgr.CheckForUpdatesAsync();

                if (newVersion != null)
                {
                    Logger.Info("Updates", $"Update available: Version {newVersion.TargetFullRelease.Version}");
                    Logger.Debug("Updates", $"Update details - Package ID: {newVersion.TargetFullRelease.PackageId}");
                }
                else
                {
                    Logger.Info("Updates", "No updates available - running latest version");
                }

                return newVersion;
            }
            catch (System.Net.WebException webEx)
            {
                Logger.Error("Updates", "Network error during update check (WebException)", webEx);
                Logger.Debug("Updates", $"WebException Status: {webEx.Status}");

                throw new Exception(
                 "Unable to connect to the update server. Please check your internet connection and try again.\n\n" +
                  "Details: Network communication failed",
                    webEx);
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                Logger.Error("Updates", "HTTP request failed during update check", httpEx);

                string userMessage = "Unable to reach the update server. Please check your internet connection and try again.";

                if (httpEx.StatusCode.HasValue)
                {
                    Logger.Warning("Updates", $"HTTP Status Code: {(int)httpEx.StatusCode.Value} ({httpEx.StatusCode.Value})");

                    userMessage += httpEx.StatusCode.Value switch
                    {
                        System.Net.HttpStatusCode.NotFound => "\n\nThe update package was not found. This may be a configuration issue.",
                        System.Net.HttpStatusCode.Forbidden => "\n\nAccess to the update server was denied. GitHub may be blocking your connection.",
                        System.Net.HttpStatusCode.ServiceUnavailable => "\n\nThe update server is temporarily unavailable. Please try again later.",
                        _ => $"\n\nHTTP Error: {(int)httpEx.StatusCode.Value}"
                    };
                }

                throw new Exception(userMessage, httpEx);
            }
            catch (TaskCanceledException timeoutEx)
            {
                Logger.Error("Updates", "Update check timed out", timeoutEx);
                Logger.Warning("Updates", "The request took too long to complete. This may indicate a slow or unstable connection.");

                throw new Exception(
                  "The update check timed out. Please check your internet connection and try again.\n\n" +
                "If you're on a slow connection, please wait and retry.",
                 timeoutEx);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                Logger.Error("Updates", "Access denied during update check", accessEx);
                throw new Exception(
              "Permission denied while checking for updates. Please run the application with appropriate permissions.",
                    accessEx);
            }
            catch (Exception ex)
            {
                Logger.Error("Updates", $"Unexpected error during update check: {ex.GetType().Name}", ex);

                throw new Exception(
                        $"Failed to check for updates: {ex.Message}\n\n" +
               $"Error Type: {ex.GetType().Name}\n\n" +
                  "Please check your internet connection or try again later.",
                   ex);
            }
        }

        public static async Task<bool> DownloadAndApplyUpdateAsync(UpdateInfo updateInfo)
        {
            Logger.Info("Updates", $"Starting download and installation of update: Version {updateInfo.TargetFullRelease.Version}");

            try
            {
                Logger.Info("Updates", "Initializing UpdateManager for download");
                var mgr = new UpdateManager(new GithubSource("https://github.com/HSP-Studios/MySchool", null, true, null));

                Logger.Info("Updates", $"Downloading update package (Version {updateInfo.TargetFullRelease.Version})...");

                // download new version
                await mgr.DownloadUpdatesAsync(updateInfo);

                Logger.Info("Updates", "Update package downloaded successfully");
                Logger.Info("Updates", "Applying update and preparing to restart application...");

                // install new version and restart app
                mgr.ApplyUpdatesAndRestart(updateInfo);

                Logger.Info("Updates", "Update applied successfully - application will restart");
                return true;
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                Logger.Error("Updates", "HTTP error during update download", httpEx);

                if (httpEx.StatusCode.HasValue)
                {
                    Logger.Warning("Updates", $"HTTP Status Code: {(int)httpEx.StatusCode.Value} ({httpEx.StatusCode.Value})");
                }

                return false;
            }
            catch (IOException ioEx)
            {
                Logger.Error("Updates", "File system error during update installation", ioEx);
                Logger.Warning("Updates", "This may be caused by insufficient disk space or permission issues");
                return false;
            }
            catch (UnauthorizedAccessException accessEx)
            {
                Logger.Error("Updates", "Permission denied during update installation", accessEx);
                Logger.Warning("Updates", "The application may need administrator privileges to update");
                return false;
            }
            catch (TaskCanceledException timeoutEx)
            {
                Logger.Error("Updates", "Update download timed out", timeoutEx);
                Logger.Warning("Updates", "The download took too long. Check your connection or try again.");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Updates", $"Unexpected error during update download/apply: {ex.GetType().Name}", ex);
                return false;
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Logger.Info("Application", "MySchool application starting...");
            Logger.Debug("Application", $"Version: {BuildInfoHelper.Version}, Build: {BuildInfoHelper.BuildNumber}");

            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string mySchoolPath = Path.Combine(appData, "MySchool");

                Logger.Info("Application", $"Ensuring application data directory exists: {mySchoolPath}");

                if (!Directory.Exists(mySchoolPath))
                {
                    Directory.CreateDirectory(mySchoolPath);
                    Logger.Info("Application", "Application data directory created successfully");
                }

                // Load settings and apply theme
                Logger.Info("Application", "Loading user settings...");
                CurrentSettings = SettingsService.Load();

                // Use ThemeName property, fallback to IsDarkMode for migration
                string themeName = CurrentSettings.ThemeName;
                if (string.IsNullOrWhiteSpace(themeName))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    themeName = CurrentSettings.IsDarkMode ? "Dark" : "Light";
#pragma warning restore CS0618 // Type or member is obsolete
                    CurrentSettings.ThemeName = themeName;
                    SettingsService.Save(CurrentSettings);
                }

                Logger.Info("Application", $"Settings loaded - Theme: {themeName}");

                Logger.Info("Application", "Applying theme...");
                ThemeManager.ApplyTheme(themeName);

                // Check for updates on startup
                await CheckForUpdatesOnStartup();

                Logger.Info("Application", "Application startup completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Application", "Error during application startup", ex);
                // Ignore any IO exceptions on startup; app can still run without the folder
            }
        }

        private async Task CheckForUpdatesOnStartup()
        {
            Logger.Info("Updates", "Performing automatic update check on startup...");

            try
            {
                var updateInfo = await CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    Logger.Info("Updates", $"Update available on startup: Version {updateInfo.TargetFullRelease.Version}");

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
                        Logger.Info("Updates", "User accepted startup update prompt");
                        bool success = await DownloadAndApplyUpdateAsync(updateInfo);

                        if (!success)
                        {
                            Logger.Warning("Updates", "Update installation failed on startup");
                            MessageBox.Show(
     "Failed to download or apply the update. Please try again later or download manually from GitHub.",
   "Update Failed",
          MessageBoxButton.OK,
               MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        Logger.Info("Updates", "User declined startup update prompt");
                    }
                }
                else
                {
                    Logger.Info("Updates", "No updates available on startup check");
                }
            }
            catch (Exception ex)
            {
                Logger.Warning("Updates", "Startup update check failed - continuing without update", ex);
                // Silently fail - don't bother user with update check errors on startup
            }
        }
    }
}
