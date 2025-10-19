using System;
using System.Collections.Generic;
using System.IO;
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
                DeveloperModeToggle.IsChecked = App.CurrentSettings.IsDeveloperMode;
                ForceDayTimeToggle.IsChecked = App.CurrentSettings.ForceDayTimeEnabled;
                
                // Set day of week
                DayOfWeekComboBox.SelectedIndex = (int)App.CurrentSettings.ForcedDayOfWeek;
                
                // Set time
                HourTextBox.Text = App.CurrentSettings.ForcedTimeOfDay.Hours.ToString("D2");
                MinuteTextBox.Text = App.CurrentSettings.ForcedTimeOfDay.Minutes.ToString("D2");
                
                UpdateLocationDisplay();
                UpdateDeveloperModeVisibility();
                UpdateForceDayTimeVisibility();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialize settings: " + ex);
                DarkModeToggle.IsChecked = false; // fallback to default
            }
        }

        private void UpdateDeveloperModeVisibility()
        {
            DeveloperOptionsPanel.Visibility = App.CurrentSettings.IsDeveloperMode 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        private void UpdateForceDayTimeVisibility()
        {
            ForceDayTimePanel.Visibility = App.CurrentSettings.ForceDayTimeEnabled 
                ? Visibility.Visible 
                : Visibility.Collapsed;
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

        private void DeveloperModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.IsDeveloperMode = true;
            SettingsService.Save(App.CurrentSettings);
            UpdateDeveloperModeVisibility();
        }

        private void DeveloperModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.IsDeveloperMode = false;
            App.CurrentSettings.ForceDayTimeEnabled = false;
            SettingsService.Save(App.CurrentSettings);
            UpdateDeveloperModeVisibility();
        }

        private void ForceDayTimeToggle_Checked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.ForceDayTimeEnabled = true;
            SettingsService.Save(App.CurrentSettings);
            UpdateForceDayTimeVisibility();
        }

        private void ForceDayTimeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.ForceDayTimeEnabled = false;
            SettingsService.Save(App.CurrentSettings);
            UpdateForceDayTimeVisibility();
        }

        private void DayOfWeekComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DayOfWeekComboBox.SelectedItem is ComboBoxItem item)
            {
                App.CurrentSettings.ForcedDayOfWeek = (DayOfWeek)DayOfWeekComboBox.SelectedIndex;
                SettingsService.Save(App.CurrentSettings);
            }
        }

        private void TimeTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                // Check if both textboxes are initialized
                if (HourTextBox == null || MinuteTextBox == null)
                    return;

                if (string.IsNullOrWhiteSpace(HourTextBox.Text) || string.IsNullOrWhiteSpace(MinuteTextBox.Text))
                    return;

                if (int.TryParse(HourTextBox.Text, out int hour) && 
                    int.TryParse(MinuteTextBox.Text, out int minute) &&
                    hour >= 0 && hour < 24 && minute >= 0 && minute < 60)
                {
                    App.CurrentSettings.ForcedTimeOfDay = new TimeSpan(hour, minute, 0);
                    SettingsService.Save(App.CurrentSettings);
                }
            }
            catch
            {
                // Invalid time input, ignore
            }
        }

        private void OpenConfigButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string configPath = System.IO.Path.Combine(SettingsService.GetDataFolderPath(), "user_settings.json");
                if (File.Exists(configPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = configPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Config file not found.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open config file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenDataFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dataFolder = SettingsService.GetDataFolderPath();
                if (Directory.Exists(dataFolder))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dataFolder,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Directory.CreateDirectory(dataFolder);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dataFolder,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open data folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
