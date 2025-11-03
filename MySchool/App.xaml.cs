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
        private static Mutex? _instanceMutex;
        private const string MutexName = "MySchool_SingleInstance_Mutex";

        // Update source URLs
        private const string GitHubRepoUrl = "https://github.com/HSP-Studios/MySchool";
        private const string S3BucketUrl = "https://myschool-ap-se-2.s3.ap-southeast-2.amazonaws.com";
        private const int UpdateCheckTimeoutSeconds = 10;

        [STAThread]
        private static void Main(string[] args)
        {
            // Check for existing instance
            bool createdNew;
            _instanceMutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                MessageBox.Show(
    "MySchool is already running. Only one instance of the application can be open at a time.",
            "Application Already Running",
        MessageBoxButton.OK,
MessageBoxImage.Information);

                // Exit this instance
                return;
            }

            try
            {
                VelopackApp.Build().Run();
                App app = new();
                app.InitializeComponent();
                app.Run();
            }
            finally
            {
                // Release the mutex when the application exits
                _instanceMutex?.ReleaseMutex();
                _instanceMutex?.Dispose();
            }
        }

        public static async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            Logger.Info("Updates", "Starting update check...");

            // Try GitHub first with timeout
            try
            {
                Logger.Info("Updates", "Initializing UpdateManager with GitHub source");
                Logger.Debug("Updates", $"Repository: {GitHubRepoUrl}, Prerelease: true, Timeout: {UpdateCheckTimeoutSeconds}s");

                var mgr = new UpdateManager(new GithubSource(GitHubRepoUrl, null, true, null));

                Logger.Info("Updates", "Checking for available updates from GitHub...");
               
                // Create a timeout task
                var checkTask = mgr.CheckForUpdatesAsync();
     var timeoutTask = Task.Delay(TimeSpan.FromSeconds(UpdateCheckTimeoutSeconds));
 
            var completedTask = await Task.WhenAny(checkTask, timeoutTask);
    
            if (completedTask == timeoutTask)
        {
        Logger.Warning("Updates", $"GitHub update check timed out after {UpdateCheckTimeoutSeconds} seconds, falling back to S3");
   throw new TimeoutException($"GitHub update check timed out after {UpdateCheckTimeoutSeconds} seconds");
                }

      var newVersion = await checkTask;

        if (newVersion != null)
           {
        Logger.Info("Updates", $"Update available from GitHub: Version {newVersion.TargetFullRelease.Version}");
        Logger.Debug("Updates", $"Update details - Package ID: {newVersion.TargetFullRelease.PackageId}");
        }
        else
          {
      Logger.Info("Updates", "No updates available from GitHub - running latest version");
       }

        return newVersion;
            }
            catch (Exception githubEx) when (
                githubEx is TimeoutException ||
        githubEx is TaskCanceledException ||
  githubEx is System.Net.WebException ||
    githubEx is System.Net.Http.HttpRequestException)
      {
      Logger.Warning("Updates", $"GitHub update check failed: {githubEx.GetType().Name} - {githubEx.Message}");
       Logger.Info("Updates", "Attempting to check updates from S3 fallback source...");

      // Try S3 fallback
      try
         {
   Logger.Info("Updates", "Initializing UpdateManager with S3 source");
        Logger.Debug("Updates", $"S3 Bucket: {S3BucketUrl}");

           var s3Mgr = new UpdateManager(new SimpleWebSource(S3BucketUrl));

       Logger.Info("Updates", "Checking for available updates from S3...");
   var newVersion = await s3Mgr.CheckForUpdatesAsync();

                 if (newVersion != null)
            {
      Logger.Info("Updates", $"Update available from S3: Version {newVersion.TargetFullRelease.Version}");
          Logger.Debug("Updates", $"Update details - Package ID: {newVersion.TargetFullRelease.PackageId}");
        }
   else
              {
         Logger.Info("Updates", "No updates available from S3 - running latest version");
        }

         return newVersion;
        }
      catch (System.Net.WebException s3WebEx)
  {
             Logger.Error("Updates", "S3 fallback also failed (WebException)", s3WebEx);
 Logger.Debug("Updates", $"WebException Status: {s3WebEx.Status}");

   throw new Exception(
             "Unable to connect to update servers (GitHub and S3 fallback).\n" +
            "Please check your internet connection and try again.\n\n" +
     "Details: Network communication failed",
          s3WebEx);
      }
       catch (System.Net.Http.HttpRequestException s3HttpEx)
             {
    Logger.Error("Updates", "S3 fallback failed (HttpRequestException)", s3HttpEx);

         string userMessage = "Unable to reach update servers (GitHub and S3 fallback).\n" +
     "Please check your internet connection and try again.";

       if (s3HttpEx.StatusCode.HasValue)
        {
            Logger.Warning("Updates", $"S3 HTTP Status Code: {(int)s3HttpEx.StatusCode.Value} ({s3HttpEx.StatusCode.Value})");
     userMessage += s3HttpEx.StatusCode.Value switch
        {
          System.Net.HttpStatusCode.NotFound => "\n\nThe update package was not found on S3.",
     System.Net.HttpStatusCode.Forbidden => "\n\nAccess to S3 was denied.",
     System.Net.HttpStatusCode.ServiceUnavailable => "\n\nThe S3 server is temporarily unavailable.",
      _ => $"\n\nHTTP Error: {(int)s3HttpEx.StatusCode.Value}"
  };
         }

        throw new Exception(userMessage, s3HttpEx);
     }
    catch (Exception s3Ex)
{
     Logger.Error("Updates", $"S3 fallback failed with unexpected error: {s3Ex.GetType().Name}", s3Ex);
                throw new Exception(
   $"Failed to check for updates from both GitHub and S3:\n\n" +
        $"{s3Ex.Message}\n\n" +
            $"Error Type: {s3Ex.GetType().Name}\n\n" +
              "Please check your internet connection or try again later.",
              s3Ex);
}
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
          
    // Try GitHub first, then S3 fallback
          UpdateManager? mgr = null;
                bool usingS3Fallback = false;
    
     try
            {
   Logger.Info("Updates", "Attempting download from GitHub");
  mgr = new UpdateManager(new GithubSource(GitHubRepoUrl, null, true, null));
            
   // Create a timeout task for download
           var downloadTask = mgr.DownloadUpdatesAsync(updateInfo);
    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(UpdateCheckTimeoutSeconds));
          
        var completedTask = await Task.WhenAny(downloadTask, timeoutTask);
           
         if (completedTask == timeoutTask)
             {
          Logger.Warning("Updates", $"GitHub download timed out after {UpdateCheckTimeoutSeconds} seconds, falling back to S3");
 throw new TimeoutException($"GitHub download timed out after {UpdateCheckTimeoutSeconds} seconds");
}
    
         await downloadTask;
           
        Logger.Info("Updates", "Update package downloaded successfully from GitHub");
    }
     catch (Exception githubDownloadEx) when (
           githubDownloadEx is TimeoutException ||
        githubDownloadEx is TaskCanceledException ||
       githubDownloadEx is System.Net.WebException ||
       githubDownloadEx is System.Net.Http.HttpRequestException)
   {
       Logger.Warning("Updates", $"GitHub download failed: {githubDownloadEx.GetType().Name}, trying S3 fallback");
            
     mgr = new UpdateManager(new SimpleWebSource(S3BucketUrl));
          usingS3Fallback = true;
             
       Logger.Info("Updates", "Attempting download from S3");
await mgr.DownloadUpdatesAsync(updateInfo);
       
         Logger.Info("Updates", "Update package downloaded successfully from S3");
   }

       Logger.Info("Updates", $"Applying update and preparing to restart application... (Source: {(usingS3Fallback ? "S3" : "GitHub")})");

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
