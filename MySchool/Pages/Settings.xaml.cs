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
    }
}
