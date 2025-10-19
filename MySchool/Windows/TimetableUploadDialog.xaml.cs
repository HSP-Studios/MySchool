using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using MySchool.Classes;

namespace MySchool.Windows
{
    public partial class TimetableUploadDialog : Window
    {
        private string? selectedPdfPath;
        private string? targetPdfPath;
        private string? targetJsonPath;

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

        private void SelectPdfButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    Title = "Select Timetable PDF"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    selectedPdfPath = openFileDialog.FileName;
                    PdfFileNameText.Text = Path.GetFileName(selectedPdfPath);
                    NextToParsing.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to select PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NextToParsing_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedPdfPath))
            {
                MessageBox.Show("Please select a PDF file first.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Generate the file paths with timestamp
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mySchoolPath = Path.Combine(appDataPath, "MySchool", "timetables");
            
            if (!Directory.Exists(mySchoolPath))
            {
                Directory.CreateDirectory(mySchoolPath);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            targetPdfPath = Path.Combine(mySchoolPath, $"{timestamp}.pdf");
            targetJsonPath = Path.Combine(mySchoolPath, $"{timestamp}.json");

            // Copy the PDF to the target location
            try
            {
                File.Copy(selectedPdfPath, targetPdfPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Switch to AI parsing panel
            PdfUploadPanel.Visibility = Visibility.Collapsed;
            AiParsingPanel.Visibility = Visibility.Visible;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back to PDF upload
            AiParsingPanel.Visibility = Visibility.Collapsed;
            PdfUploadPanel.Visibility = Visibility.Visible;

            // Clean up the copied PDF if going back
            if (!string.IsNullOrEmpty(targetPdfPath) && File.Exists(targetPdfPath))
            {
                try
                {
                    File.Delete(targetPdfPath);
                }
                catch { /* Ignore deletion errors */ }
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

                // Parse and validate JSON
                TimetableData? timetableData;
                try
                {
                    timetableData = JsonSerializer.Deserialize<TimetableData>(jsonResponse);
                    
                    if (timetableData == null || timetableData.Timetable == null || !timetableData.Timetable.Any())
                    {
                        MessageBox.Show("Invalid JSON format: 'timetable' property not found or empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch (JsonException)
                {
                    MessageBox.Show("Invalid JSON format. Please ensure you copied the complete JSON response from the AI.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check for subject names longer than 8 characters (excluding breaks)
                var longSubjects = GetLongSubjectNames(timetableData);

                if (longSubjects.Any())
                {
                    // Show subject shortener dialog
                    var shortenerDialog = new SubjectShortenerDialog(longSubjects)
                    {
                        Owner = this
                    };

                    if (shortenerDialog.ShowDialog() == true)
                    {
                        // Apply shortened names to timetable data
                        ApplyShortenedSubjectNames(timetableData, shortenerDialog.ShortenedSubjects);
                    }
                    else
                    {
                        // User cancelled, don't save
                        return;
                    }
                }

                // Write formatted JSON
                if (string.IsNullOrEmpty(targetJsonPath))
                {
                    MessageBox.Show("Error: Target path not set.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string formattedJson = JsonSerializer.Serialize(timetableData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(targetJsonPath, formattedJson);

                MessageBox.Show($"Timetable saved successfully!\n\nPDF: {targetPdfPath}\nJSON: {targetJsonPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save timetable: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<string> GetLongSubjectNames(TimetableData timetableData)
        {
            var longSubjects = new HashSet<string>();

            foreach (var day in timetableData.Timetable)
            {
                foreach (var period in day.Periods)
                {
                    // Skip breaks and subjects with 8 or fewer characters
                    if (!period.IsBreak && !string.IsNullOrWhiteSpace(period.Subject) && period.Subject.Length > 8)
                    {
                        longSubjects.Add(period.Subject);
                    }
                }
            }

            return longSubjects.OrderBy(s => s).ToList();
        }

        private void ApplyShortenedSubjectNames(TimetableData timetableData, Dictionary<string, string> shortenedNames)
        {
            foreach (var day in timetableData.Timetable)
            {
                foreach (var period in day.Periods)
                {
                    if (shortenedNames.ContainsKey(period.Subject))
                    {
                        period.Subject = shortenedNames[period.Subject];
                    }
                }
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
