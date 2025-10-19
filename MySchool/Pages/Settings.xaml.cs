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
                UpdateLocationDisplay();
                
                // Initialize developer mode settings
                DeveloperModeToggle.IsChecked = App.CurrentSettings.DeveloperMode;
                DeveloperSettings.Visibility = App.CurrentSettings.DeveloperMode ? Visibility.Visible : Visibility.Collapsed;
                
                // Load developer settings
                LoadDeveloperSettings();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialize settings: " + ex);
                DarkModeToggle.IsChecked = false; // fallback to default
            }
        }

        private void LoadDeveloperSettings()
        {
            // Layout mode
            var layoutMode = App.CurrentSettings.ForceLayoutMode;
            foreach (ComboBoxItem item in LayoutModeComboBox.Items)
            {
                if (item.Tag.ToString() == layoutMode)
                {
                    LayoutModeComboBox.SelectedItem = item;
                    break;
                }
            }

            // Weather override
            ForceWeatherToggle.IsChecked = App.CurrentSettings.ForceWeatherEnabled;
            WeatherOverrideSettings.Visibility = App.CurrentSettings.ForceWeatherEnabled ? Visibility.Visible : Visibility.Collapsed;

            // Weather settings
            var condition = App.CurrentSettings.ForcedWeatherCondition;
            foreach (ComboBoxItem item in WeatherConditionComboBox.Items)
            {
                if (item.Tag.ToString() == condition)
                {
                    WeatherConditionComboBox.SelectedItem = item;
                    break;
                }
            }

            TemperatureTextBox.Text = App.CurrentSettings.ForcedTemperature.ToString();
            WeatherDescriptionTextBox.Text = App.CurrentSettings.ForcedWeatherDescription;
            LocationNameTextBox.Text = App.CurrentSettings.ForcedLocationName;
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

        // Developer Mode Event Handlers
        private void DeveloperModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.DeveloperMode = true;
            SettingsService.Save(App.CurrentSettings);
            DeveloperSettings.Visibility = Visibility.Visible;
        }

        private void DeveloperModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.DeveloperMode = false;
            SettingsService.Save(App.CurrentSettings);
            DeveloperSettings.Visibility = Visibility.Collapsed;
        }

        private void LayoutModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LayoutModeComboBox.SelectedItem is ComboBoxItem item)
            {
                App.CurrentSettings.ForceLayoutMode = item.Tag.ToString();
                SettingsService.Save(App.CurrentSettings);
            }
        }

        private void ForceWeatherToggle_Checked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.ForceWeatherEnabled = true;
            SettingsService.Save(App.CurrentSettings);
            WeatherOverrideSettings.Visibility = Visibility.Visible;
        }

        private void ForceWeatherToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            App.CurrentSettings.ForceWeatherEnabled = false;
            SettingsService.Save(App.CurrentSettings);
            WeatherOverrideSettings.Visibility = Visibility.Collapsed;
        }

        private void WeatherConditionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (WeatherConditionComboBox.SelectedItem is ComboBoxItem item)
            {
                App.CurrentSettings.ForcedWeatherCondition = item.Tag.ToString();
                SettingsService.Save(App.CurrentSettings);
            }
        }

        private void TemperatureTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (double.TryParse(TemperatureTextBox.Text, out double temp))
            {
                App.CurrentSettings.ForcedTemperature = temp;
                SettingsService.Save(App.CurrentSettings);
            }
        }

        private void WeatherDescriptionTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            App.CurrentSettings.ForcedWeatherDescription = WeatherDescriptionTextBox.Text;
            SettingsService.Save(App.CurrentSettings);
        }

        private void LocationNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            App.CurrentSettings.ForcedLocationName = LocationNameTextBox.Text;
            SettingsService.Save(App.CurrentSettings);
        }

        private void OpenConfigButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string mySchoolPath = System.IO.Path.Combine(appData, "MySchool");
                string configPath = System.IO.Path.Combine(mySchoolPath, "user_settings.json");

                if (System.IO.File.Exists(configPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = configPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Config file not found. It will be created on next save.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
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
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string mySchoolPath = System.IO.Path.Combine(appData, "MySchool");

                // Create directory if it doesn't exist
                if (!System.IO.Directory.Exists(mySchoolPath))
                {
                    System.IO.Directory.CreateDirectory(mySchoolPath);
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = mySchoolPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open data folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
