using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySchool.Windows;
using MySchool.Classes;

namespace MySchool.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            Loaded += Settings_Loaded;
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize toggle from current settings
            try
            {
                DarkModeToggle.IsChecked = App.CurrentSettings.IsDarkMode;
                UserNameTextBox.Text = App.CurrentSettings.UserName;
                UpdateLocationDisplay();
                UpdateBuildInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialize settings: " + ex);
                DarkModeToggle.IsChecked = false; // fallback to default
            }
        }

        private void UpdateLocationDisplay()
        {
            if (App.CurrentSettings.WeatherLocation.HasValue)
            {
                var loc = App.CurrentSettings.WeatherLocation.Value;
                var name = string.IsNullOrWhiteSpace(App.CurrentSettings.WeatherLocationName) 
                    ? "Manual location" 
                    : App.CurrentSettings.WeatherLocationName;
                CurrentLocationText.Text = $"{name} ({loc.latitude:F2}, {loc.longitude:F2})";
            }
            else
            {
                CurrentLocationText.Text = "Using device location";
            }
        }

        private void UpdateBuildInfo()
        {
            try
            {
                VersionText.Text = BuildInfoHelper.Version;
                BuildNumberText.Text = BuildInfoHelper.BuildNumber;
                BuildDateText.Text = BuildInfoHelper.BuildDate;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load build info: " + ex);
                VersionText.Text = "Unknown";
                BuildNumberText.Text = "Unknown";
                BuildDateText.Text = "Unknown";
            }
        }

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            SetDarkMode(true);
        }

        private void DarkModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SetDarkMode(false);
        }

        private void SetDarkMode(bool isDark)
        {
            App.CurrentSettings.IsDarkMode = isDark;
            SettingsService.Save(App.CurrentSettings);
            // Apply theme with cross-fade transition (0.5s)
            ThemeManager.ApplyThemeWithTransition(isDark, 0.5);
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                App.CurrentSettings.UserName = UserNameTextBox.Text.Trim();
                SettingsService.Save(App.CurrentSettings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to save user name: " + ex);
            }
        }

        private void UploadTimetableButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new TimetableUploadDialog
                {
                    Owner = Window.GetWindow(this)
                };
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open upload dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RequestLocationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var location = await WeatherService.GetLocationAsync();
                
                if (location.HasValue)
                {
                    // Clear manual location to use device location
                    App.CurrentSettings.WeatherLocation = null;
                    App.CurrentSettings.WeatherLocationName = string.Empty;
                    SettingsService.Save(App.CurrentSettings);
                    UpdateLocationDisplay();
                    
                    MessageBox.Show($"Location permission granted!\nYour coordinates: {location.Value.latitude:F4}, {location.Value.longitude:F4}", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Unable to access location. Please check your location permissions in Windows Settings.", 
                        "Location Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to request location: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManualLocationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ManualLocationDialog
                {
                    Owner = Window.GetWindow(this)
                };
                
                if (dialog.ShowDialog() == true)
                {
                    UpdateLocationDisplay();
                    MessageBox.Show("Location saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open location dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CheckUpdatesButton_Click(object sender, RoutedEventArgs e)
        {
            var checkingDialog = new Window
            {
                Title = "Checking for Updates",
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow,
                Background = (Brush)Application.Current.Resources["Brush.Background"]
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var progressBar = new ProgressBar
            {
                IsIndeterminate = true,
                Width = 300,
                Height = 20,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var statusText = new TextBlock
            {
                Text = "Checking for updates...",
                HorizontalAlignment = HorizontalAlignment.Center,
                Style = (Style)Application.Current.Resources["Text.Body"]
            };

            stackPanel.Children.Add(progressBar);
            stackPanel.Children.Add(statusText);
            checkingDialog.Content = stackPanel;

            // Track dialog state
            bool dialogClosed = false;
            checkingDialog.Closed += (s, args) => dialogClosed = true;

            // Disable the button while checking
            CheckUpdatesButton.IsEnabled = false;

            // Show the dialog non-modally
            checkingDialog.Show();

            // Helper method to safely close the dialog
            void SafeCloseDialog()
            {
                try
                {
                    if (!dialogClosed && checkingDialog.IsLoaded)
                    {
                        checkingDialog.Close();
                        dialogClosed = true;
                    }
                }
                catch (Exception)
                {
                    // Ignore errors when closing dialog
                }
            }

            try
            {
                var updateInfo = await App.CheckForUpdatesAsync();

                SafeCloseDialog();

                if (updateInfo != null)
                {
                    var result = MessageBox.Show(
                        $"A new version of MySchool is available!\n\n" +
                        $"New Version: {updateInfo.TargetFullRelease.Version}\n\n" +
                        $"Would you like to download and install the update now?\n" +
                        $"The application will restart after the update.",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        statusText.Text = "Downloading update...";
                        checkingDialog.Show();
                        dialogClosed = false;
                        checkingDialog.Closed += (s, args) => dialogClosed = true;

                        bool success = await App.DownloadAndApplyUpdateAsync(updateInfo);
 
                        // Close dialog BEFORE restart to prevent the error
                        SafeCloseDialog();

                        if (!success)
                        {
                            MessageBox.Show(
                                "Failed to download or apply the update. Please try again later or download manually from GitHub.",
                                "Update Failed",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                        // If success, app will restart automatically and we won't reach here
                    }
                }
                else
                {
                    MessageBox.Show(
                        "You are running the latest version of MySchool!",
                        "No Updates Available",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                SafeCloseDialog();
                
                // Provide detailed error message based on exception type
                string errorMessage = ex.Message;
        
                // Add helpful suggestions based on the error
                if (errorMessage.Contains("internet connection") || 
                     errorMessage.Contains("timed out") || 
                     errorMessage.Contains("connect"))
                {
                    errorMessage += "\n\nPlease ensure:\n" +
                                   "• You are connected to the internet\n" +
                                   "• Your firewall isn't blocking the connection\n" +
                                   "• GitHub.com is accessible from your network";
                }
        
                MessageBox.Show(
                    errorMessage,
                    "Update Check Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                CheckUpdatesButton.IsEnabled = true;
            }
        }
    }
}
