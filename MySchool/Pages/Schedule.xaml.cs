using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MySchool.Classes;

namespace MySchool.Pages
{
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class Schedule : Page
    {
        private string? currentTimetablePath;

        public Schedule()
        {
            InitializeComponent();
            Loaded += Schedule_Loaded;
        }

        private void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTimetablePdf();
        }

        private void LoadTimetablePdf()
        {
            try
            {
                var pdfPath = TimetableManager.GetLatestTimetablePdf();
                
                if (!string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                {
                    currentTimetablePath = pdfPath;
                    
                    // Show timetable available panel, hide no timetable message
                    NoTimetablePanel.Visibility = Visibility.Collapsed;
                    TimetableAvailablePanel.Visibility = Visibility.Visible;
                    
                    // Display the filename
                    TimetableFileNameText.Text = Path.GetFileName(pdfPath);
                }
                else
                {
                    // Show no timetable message, hide timetable panel
                    NoTimetablePanel.Visibility = Visibility.Visible;
                    TimetableAvailablePanel.Visibility = Visibility.Collapsed;
                    currentTimetablePath = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load PDF: {ex.Message}");
                NoTimetablePanel.Visibility = Visibility.Visible;
                TimetableAvailablePanel.Visibility = Visibility.Collapsed;
                currentTimetablePath = null;
            }
        }

        private void OpenTimetableButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentTimetablePath) && File.Exists(currentTimetablePath))
                {
                    // Open the PDF with the default system application
                    var psi = new ProcessStartInfo
                    {
                        FileName = currentTimetablePath,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                else
                {
                    MessageBox.Show("Timetable file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open timetable: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
