using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace MySchool.Windows
{
    public partial class TimetableUploadDialog : Window
    {
        public TimetableUploadDialog()
        {
            InitializeComponent();
            LoadPrompt();
        }

        private void LoadPrompt()
        {
            try
            {
                // Load the prompt from resources
                string promptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "prompts", "timetable-generation-prompt.txt");
                if (File.Exists(promptPath))
                {
                    PromptTextBox.Text = File.ReadAllText(promptPath);
                }
                else
                {
                    PromptTextBox.Text = "Error: Prompt file not found.";
                }
            }
            catch (Exception ex)
            {
                PromptTextBox.Text = $"Error loading prompt: {ex.Message}";
            }
        }

        private void CopyPromptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(PromptTextBox.Text);
                CopyPromptButton.Content = "? Copied!";
                
                // Reset button text after 2 seconds
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(2)
                };
                timer.Tick += (s, args) =>
                {
                    CopyPromptButton.Content = "Copy Prompt";
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy prompt: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonResponse = ResponseTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(jsonResponse))
                {
                    MessageBox.Show("Please paste the AI response before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate JSON
                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        // Basic validation - check if timetable array exists
                        if (!doc.RootElement.TryGetProperty("timetable", out _))
                        {
                            MessageBox.Show("Invalid JSON format: 'timetable' property not found.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
                catch (JsonException)
                {
                    MessageBox.Show("Invalid JSON format. Please ensure you copied the complete JSON response from the AI.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Save to file
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string mySchoolPath = Path.Combine(appDataPath, "MySchool", "timetables");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(mySchoolPath))
                {
                    Directory.CreateDirectory(mySchoolPath);
                }

                // Generate filename with current date and time
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                string filename = $"{timestamp}.json";
                string filePath = Path.Combine(mySchoolPath, filename);

                // Write formatted JSON
                var jsonDoc = JsonDocument.Parse(jsonResponse);
                string formattedJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, formattedJson);

                MessageBox.Show($"Timetable saved successfully!\n\nLocation: {filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save timetable: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
