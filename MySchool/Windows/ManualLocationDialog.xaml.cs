using MySchool.Classes;
using System;
using System.Globalization;
using System.Windows;

namespace MySchool.Windows
{
    public partial class ManualLocationDialog : Window
    {
        public (double latitude, double longitude)? Location { get; private set; }
        public string LocationName { get; private set; } = string.Empty;

        public ManualLocationDialog()
        {
            InitializeComponent();
            
            // Load existing location if available
            if (App.CurrentSettings.WeatherLocation.HasValue)
            {
                var loc = App.CurrentSettings.WeatherLocation.Value;
                LatitudeTextBox.Text = loc.latitude.ToString(CultureInfo.InvariantCulture);
                LongitudeTextBox.Text = loc.longitude.ToString(CultureInfo.InvariantCulture);
                LocationNameTextBox.Text = App.CurrentSettings.WeatherLocationName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(LatitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude) ||
                    !double.TryParse(LongitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude))
                {
                    MessageBox.Show("Please enter valid latitude and longitude values.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate coordinates
                if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                {
                    MessageBox.Show("Latitude must be between -90 and 90.\nLongitude must be between -180 and 180.", "Invalid Coordinates", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Location = (latitude, longitude);
                LocationName = LocationNameTextBox.Text.Trim();

                // Save to settings
                App.CurrentSettings.WeatherLocation = Location;
                App.CurrentSettings.WeatherLocationName = LocationName;
                SettingsService.Save(App.CurrentSettings);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save location: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
